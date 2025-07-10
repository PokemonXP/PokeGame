using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using PokeGame.Api.Extensions;
using PokeGame.Api.Settings;

namespace PokeGame.Api;

internal class ExceptionHandler : IExceptionHandler
{
  private readonly ErrorSettings _errorSettings;
  private readonly ProblemDetailsFactory _problemDetailsFactory;
  private readonly IProblemDetailsService _problemDetailsService;

  public ExceptionHandler(ErrorSettings errorSettings, ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
  {
    _errorSettings = errorSettings;
    _problemDetailsFactory = problemDetailsFactory;
    _problemDetailsService = problemDetailsService;
  }

  public virtual async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    int? statusCode = null;
    if (IsBadRequest(exception))
    {
      statusCode = StatusCodes.Status400BadRequest;
    }
    else if (exception is NotFoundException)
    {
      statusCode = StatusCodes.Status404NotFound;
    }
    else if (exception is ConflictException)
    {
      statusCode = StatusCodes.Status409Conflict;
    }
    else if (_errorSettings.ExposeDetail)
    {
      statusCode = StatusCodes.Status500InternalServerError;
    }

    if (statusCode is null)
    {
      return false;
    }

    Error error = ToError(exception);
    ProblemDetails problemDetails = _problemDetailsFactory.CreateProblemDetails(httpContext, statusCode.Value, error);

    httpContext.Response.StatusCode = statusCode.Value;
    ProblemDetailsContext context = new()
    {
      HttpContext = httpContext,
      ProblemDetails = problemDetails,
      Exception = exception
    };
    return await _problemDetailsService.TryWriteAsync(context);
  }

  private static bool IsBadRequest(Exception exception) => exception is BadRequestException
    || exception is InvalidCredentialsException
    || exception is SecurityTokenArgumentException
    || exception is SecurityTokenException
    || exception is TooManyResultsException
    || exception is ValidationException;

  private static Error ToError(Exception exception)
  {
    Error error;
    if (exception is ErrorException errorException)
    {
      error = errorException.Error;
    }
    else if (exception is ValidationException validation)
    {
      error = new(exception.GetErrorCode(), "Validation failed.");
      error.Data["Failures"] = validation.Errors;
    }
    else
    {
      error = new(exception);
    }
    return error;
  }
}
