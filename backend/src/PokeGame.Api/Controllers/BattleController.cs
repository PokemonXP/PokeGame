using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Battle;
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

  [HttpDelete("{id}")]
  public async Task<ActionResult<BattleModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    BattleModel? battle = await _battleService.DeleteAsync(id, cancellationToken);
    return battle is null ? NotFound() : Ok(battle);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<BattleModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    BattleModel? battle = await _battleService.ReadAsync(id, cancellationToken);
    return battle is null ? NotFound() : Ok(battle);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<BattleModel>>> SearchAsync([FromQuery] SearchBattlesParameters parameters, CancellationToken cancellationToken)
  {
    SearchBattlesPayload payload = parameters.ToPayload();
    SearchResults<BattleModel> battles = await _battleService.SearchAsync(payload, cancellationToken);
    return Ok(battles);
  }

  [HttpPatch("{id}/start")]
  public async Task<ActionResult<BattleModel>> StartAsync(Guid id, CancellationToken cancellationToken)
  {
    BattleModel? battle = await _battleService.StartAsync(id, cancellationToken);
    return battle is null ? NotFound() : Ok(battle);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<BattleModel>> UpdateAsync(Guid id, UpdateBattlePayload payload, CancellationToken cancellationToken)
  {
    BattleModel? battle = await _battleService.UpdateAsync(id, payload, cancellationToken);
    return battle is null ? NotFound() : Ok(battle);
  }
}
