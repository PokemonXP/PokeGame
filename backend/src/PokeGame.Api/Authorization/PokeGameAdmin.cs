using Logitar.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using PokeGame.Api.Constants;

namespace PokeGame.Api.Authorization;

internal class PokeGameAdminRequirement : IAuthorizationRequirement;

internal class PokeGameAdminHandler : AuthorizationHandler<PokeGameAdminRequirement>
{
  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PokeGameAdminRequirement requirement)
  {
    IEnumerable<Claim> claims = context.User.FindAll(Rfc7519ClaimNames.Roles);
    if (claims.Any(claim => claim.Value.Equals(Roles.Admin, StringComparison.InvariantCultureIgnoreCase)))
    {
      context.Succeed(requirement);
    }

    return Task.CompletedTask;
  }
}
