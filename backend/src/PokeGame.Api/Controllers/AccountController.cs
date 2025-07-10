using FluentValidation;
using Google.Apis.Auth;
using Krakenar.Client;
using Krakenar.Contracts;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Authentication;
using PokeGame.Api.Extensions;
using PokeGame.Api.Models.Account;
using PokeGame.Api.Settings;

namespace PokeGame.Api.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly ErrorSettings _errorSettings;
  private readonly GoogleSettings _googleSettings;
  private readonly ILogger<AccountController> _logger;
  private readonly IOpenAuthenticationService _openAuthenticationService;
  private readonly ISessionService _sessionService;
  private readonly IUserService _userService;

  public AccountController(
    ErrorSettings errorSettings,
    GoogleSettings googleSettings,
    ILogger<AccountController> logger,
    IOpenAuthenticationService openAuthenticationService,
    ISessionService sessionService,
    IUserService userService)
  {
    _errorSettings = errorSettings;
    _googleSettings = googleSettings;
    _logger = logger;
    _openAuthenticationService = openAuthenticationService;
    _sessionService = sessionService;
    _userService = userService;
  }

  [HttpPost("/auth/token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] GetTokenPayload input, CancellationToken cancellationToken)
  {
    new GetTokenValidator().ValidateAndThrow(input);

    try
    {
      Session session;
      if (!string.IsNullOrWhiteSpace(input.RefreshToken))
      {
        RenewSessionPayload payload = new(input.RefreshToken, HttpContext.GetSessionCustomAttributes());
        session = await _sessionService.RenewAsync(payload, cancellationToken);
      }
      else if (input.Credentials is not null)
      {
        SignInSessionPayload payload = new(input.Credentials.Username, input.Credentials.Password, isPersistent: true, HttpContext.GetSessionCustomAttributes());
        session = await _sessionService.SignInAsync(payload, cancellationToken);
      }
      else
      {
        throw new ArgumentException($"Exactly one of the following properties must be specified: {nameof(input.RefreshToken)}, {nameof(input.Credentials)}.", nameof(input));
      }

      TokenResponse response = await _openAuthenticationService.GetTokenResponseAsync(session, cancellationToken);
      return Ok(response);
    }
    catch (KrakenarClientException exception)
    {
      if (_errorSettings.ExposeDetail)
      {
        throw;
      }

      string serializedError = JsonSerializer.Serialize(exception.Error);
      _logger.LogError(exception, "Invalid credentials: {Error}", serializedError);

      Error error = new("InvalidCredentials", "The specified credentials did not match.");
      return Problem(
        detail: error.Message,
        instance: Request.GetDisplayUrl(),
        statusCode: StatusCodes.Status400BadRequest,
        title: "Invalid Credentials",
        type: null,
        extensions: new Dictionary<string, object?> { ["error"] = error });
    }
  }

  [HttpGet("/profile")]
  [Authorize]
  public async Task<ActionResult<UserProfile>> GetProfileAsync(CancellationToken cancellationToken)
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
    user = await _userService.ReadAsync(user.Id, uniqueName: null, customIdentifier: null, cancellationToken)
      ?? throw new InvalidOperationException($"The user 'Id={user.Id}' was not found.");
    UserProfile profile = new(user);
    return Ok(profile);
  }

  [HttpPost("/sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload input, CancellationToken cancellationToken)
  {
    new SignInValidator().ValidateAndThrow(input);

    try
    {
      SignInSessionPayload payload = new(input.Username, input.Password, isPersistent: true, HttpContext.GetSessionCustomAttributes());
      Session session = await _sessionService.SignInAsync(payload, cancellationToken);
      HttpContext.SignIn(session);

      CurrentUser currentUser = new(session);
      return Ok(currentUser);
    }
    catch (KrakenarClientException exception)
    {
      if (_errorSettings.ExposeDetail)
      {
        throw;
      }

      string serializedError = JsonSerializer.Serialize(exception.Error);
      _logger.LogError(exception, "Invalid credentials: {Error}", serializedError);

      Error error = new("InvalidCredentials", "The specified credentials did not match.");
      return Problem(
        detail: error.Message,
        instance: Request.GetDisplayUrl(),
        statusCode: StatusCodes.Status400BadRequest,
        title: "Invalid Credentials",
        type: null,
        extensions: new Dictionary<string, object?> { ["error"] = error });
    }
  }

  [HttpPost("/sign/in/google")]
  public async Task<ActionResult<CurrentUser>> SignInGoogleAsync(GoogleSignInPayload input, CancellationToken cancellationToken)
  {
    new GoogleSignInValidator().ValidateAndThrow(input);

    try
    {
      GoogleJsonWebSignature.ValidationSettings settings = new()
      {
        Audience = [_googleSettings.ClientId]
      };
      GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(input.Token, settings);

      User? user = await _userService.ReadAsync(id: null, uniqueName: null, new CustomIdentifier(_googleSettings.IdentifierKey, payload.Subject), cancellationToken)
        ?? await _userService.ReadAsync(id: null, payload.Email, customIdentifier: null, cancellationToken);
      if (user is null)
      {
        CreateOrReplaceUserPayload createPayload = new(payload.Email)
        {
          Email = new EmailPayload(payload.Email, payload.EmailVerified),
          FirstName = payload.GivenName,
          LastName = payload.FamilyName,
          Locale = payload.Locale,
          Picture = payload.Picture
        };
        CreateOrReplaceUserResult createResult = await _userService.CreateOrReplaceAsync(createPayload, id: null, version: null, cancellationToken);
        user = createResult.User ?? throw new InvalidOperationException("The user creation failed.");
      }
      else
      {
        int changes = 0;
        UpdateUserPayload updates = new();
        if (!string.IsNullOrWhiteSpace(payload.Email)
          && (user.Email is null || (user.Email.Address.Equals(payload.Email.Trim(), StringComparison.InvariantCultureIgnoreCase) && !user.Email.IsVerified && payload.EmailVerified)))
        {
          EmailPayload email = new(user.Email?.Address ?? payload.Email, payload.EmailVerified);
          updates.Email = new Change<EmailPayload>(email);
          changes++;
        }
        if (user.FirstName is null && !string.IsNullOrWhiteSpace(payload.GivenName))
        {
          updates.FirstName = new Change<string>(payload.GivenName);
          changes++;
        }
        if (user.LastName is null && !string.IsNullOrWhiteSpace(payload.FamilyName))
        {
          updates.LastName = new Change<string>(payload.FamilyName);
          changes++;
        }
        if (user.Locale is null && !string.IsNullOrWhiteSpace(payload.Locale))
        {
          updates.Locale = new Change<string>(payload.Locale);
          changes++;
        }
        if (user.Picture is null && !string.IsNullOrWhiteSpace(payload.Picture))
        {
          updates.Picture = new Change<string>(payload.Picture);
          changes++;
        }
        if (changes > 0)
        {
          user = await _userService.UpdateAsync(user.Id, updates, cancellationToken)
            ?? throw new InvalidOperationException($"The user 'Id={user.Id}' update returned null.");
        }
      }
      if (!user.CustomIdentifiers.Any(identifier => identifier.Key.Equals(_googleSettings.IdentifierKey, StringComparison.InvariantCultureIgnoreCase)))
      {
        SaveUserIdentifierPayload identifierPayload = new(payload.Subject);
        user = await _userService.SaveIdentifierAsync(user.Id, _googleSettings.IdentifierKey, identifierPayload, cancellationToken)
          ?? throw new InvalidOperationException($"The user 'Id={user.Id}' identifier save returned null.");
      }

      CreateSessionPayload createSession = new(user.Id.ToString(), isPersistent: true, HttpContext.GetSessionCustomAttributes());
      Session session = await _sessionService.CreateAsync(createSession, cancellationToken);
      HttpContext.SignIn(session);

      CurrentUser currentUser = new(session);
      return Ok(currentUser);
    }
    catch (InvalidJwtException exception)
    {
      if (_errorSettings.ExposeDetail)
      {
        throw;
      }

      _logger.LogError(exception, "Google ID Token validation failed.");

      Error error = new("InvalidCredentials", "The specified credentials did not match.");
      return Problem(
        detail: error.Message,
        instance: Request.GetDisplayUrl(),
        statusCode: StatusCodes.Status400BadRequest,
        title: "Invalid Credentials",
        type: null,
        extensions: new Dictionary<string, object?> { ["error"] = error });
    }
  }

  [HttpPost("/sign/out")]
  public async Task<ActionResult> SignOutAsync(bool everywhere, CancellationToken cancellationToken)
  {
    if (everywhere)
    {
      User? user = HttpContext.GetUser();
      if (user is not null)
      {
        await _userService.SignOutAsync(user.Id, cancellationToken);
      }
    }
    else
    {
      Guid? sessionId = HttpContext.GetSessionId();
      if (sessionId.HasValue)
      {
        await _sessionService.SignOutAsync(sessionId.Value, cancellationToken);
      }
    }
    return NoContent();
  }
}
