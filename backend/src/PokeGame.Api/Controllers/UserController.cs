using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.User;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("users")]
public class UserController : ControllerBase
{
  private readonly IUserService _userService;

  public UserController(IUserService userService)
  {
    _userService = userService;
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<UserSummary>>> SearchAsync(CancellationToken cancellationToken)
  {
    SearchResults<User> users = await _userService.SearchAsync(new SearchUsersPayload(), cancellationToken);
    SearchResults<UserSummary> summaries = new(users.Items.Select(user => new UserSummary(user)), users.Total);
    return Ok(summaries);
  }
}
