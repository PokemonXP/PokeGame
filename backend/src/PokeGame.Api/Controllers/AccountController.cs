using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Authentication;
using PokeGame.Api.Extensions;
using PokeGame.Api.Models.Account;
using PokeGame.Api.Settings;
using System.Text.Json;

namespace PokeGame.Api.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly ErrorSettings _errorSettings;
  private readonly ILogger<AccountController> _logger;
  private readonly IOpenAuthenticationService _openAuthenticationService;
  private readonly ISessionService _sessionService;
  private readonly IUserService _userService;

  public AccountController(
    ErrorSettings errorSettings,
    ILogger<AccountController> logger,
    IOpenAuthenticationService openAuthenticationService,
    ISessionService sessionService,
    IUserService userService)
  {
    _errorSettings = errorSettings;
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
    catch (InvalidCredentialsException exception) // TODO(fpion): probably won't work.
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
    catch (InvalidCredentialsException exception) // TODO(fpion): probably won't work.
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
