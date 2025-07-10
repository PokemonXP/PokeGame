using Krakenar.Contracts;
using Logitar;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace PokeGame.Api.Extensions;

internal static class ProblemDetailsExtensions
{
  public static ProblemDetails CreateProblemDetails(this ProblemDetailsFactory factory, HttpContext httpContext, int statusCode, Error error)
  {
    ProblemDetails problemDetails = factory.CreateProblemDetails(
      httpContext,
      statusCode,
      title: error.Code.Humanize(),
      type: null,
      detail: error.Message,
      instance: httpContext.Request.GetDisplayUrl());

    problemDetails.Extensions["error"] = error;

    return problemDetails;
  }
}
