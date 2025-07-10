using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Sessions;
using Krakenar.Core;
using Krakenar.Web;
using Krakenar.Web.Authentication;
using Krakenar.Web.Models.Authentication;
using Krakenar.Web.Settings;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Cms.Models.Account;

namespace PokeGame.Cms.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly ErrorSettings _errorSettings;
  private readonly ILogger<AccountController> _logger;
  private readonly IOpenAuthenticationService _openAuthenticationService;
  private readonly ISessionService _sessionService;

  public AccountController(
    ErrorSettings errorSettings,
    ILogger<AccountController> logger,
    IOpenAuthenticationService openAuthenticationService,
    ISessionService sessionService)
  {
    _errorSettings = errorSettings;
    _logger = logger;
    _openAuthenticationService = openAuthenticationService;
    _sessionService = sessionService;
  }

  [HttpPost("api/auth/token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] GetTokenPayload input, CancellationToken cancellationToken)
  {
    new GetTokenValidator().ValidateAndThrow(input);

    try
    {
      Session session;
      if (!string.IsNullOrWhiteSpace(input.RefreshToken))
      {
        RenewSessionPayload payload = new(input.RefreshToken.Trim(), HttpContext.GetSessionCustomAttributes());
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
    catch (InvalidCredentialsException exception)
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

  [HttpPost("api/sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInAccountPayload input, CancellationToken cancellationToken)
  {
    new SignInAccountValidator().ValidateAndThrow(input);

    try
    {
      SignInSessionPayload payload = new(input.Username, input.Password, isPersistent: true, HttpContext.GetSessionCustomAttributes());
      Session session = await _sessionService.SignInAsync(payload, cancellationToken);
      HttpContext.SignIn(session);

      CurrentUser currentUser = new(session);
      return Ok(currentUser);
    }
    catch (InvalidCredentialsException exception)
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
}
