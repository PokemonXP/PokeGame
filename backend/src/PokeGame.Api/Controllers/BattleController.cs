using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Core.Battles;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("battles")]
public class BattleController : ControllerBase
{
  private readonly IBattleService _battleService;

  public BattleController(IBattleService battleService)
  {
    _battleService = battleService;
  }

  [HttpPost]
  public async Task<ActionResult<BattleModel>> CreateAsync([FromBody] CreateBattlePayload payload, CancellationToken cancellationToken)
  {
    BattleModel battle = await _battleService.CreateAsync(payload, cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/battles/{battle.Id}");
    return Created(location, battle);
  }
}
