using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Ability;
using PokeGame.Core.Abilities;
using PokeGame.Core.Abilities.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("api/abilities")]
public class AbilityController : ControllerBase
{
  private readonly IAbilityQuerier _abilityQuerier;

  public AbilityController(IAbilityQuerier abilityQuerier)
  {
    _abilityQuerier = abilityQuerier;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AbilityModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    AbilityModel? ability = await _abilityQuerier.ReadAsync(id, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<AbilityModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    AbilityModel? ability = await _abilityQuerier.ReadAsync(uniqueName, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<AbilityModel>>> SearchAsync([FromQuery] SearchAbilitiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchAbilitiesPayload payload = parameters.ToPayload();
    SearchResults<AbilityModel> abilities = await _abilityQuerier.SearchAsync(payload, cancellationToken);
    return Ok(abilities);
  }
}
