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
[Route("abilities")]
public class AbilityController : ControllerBase
{
  private readonly IAbilityService _abilityService;

  public AbilityController(IAbilityService abilityService)
  {
    _abilityService = abilityService;
  }

  [HttpPost]
  public async Task<ActionResult<AbilityModel>> CreateAsync(CreateOrReplaceAbilityPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceAbilityResult result = await _abilityService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AbilityModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    AbilityModel? ability = await _abilityService.ReadAsync(id, uniqueName: null, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<AbilityModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    AbilityModel? ability = await _abilityService.ReadAsync(id: null, uniqueName, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<AbilityModel>> ReplaceAsync(Guid id, CreateOrReplaceAbilityPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceAbilityResult result = await _abilityService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<AbilityModel>>> SearchAsync([FromQuery] SearchAbilitiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchAbilitiesPayload payload = parameters.ToPayload();
    SearchResults<AbilityModel> abilities = await _abilityService.SearchAsync(payload, cancellationToken);
    return Ok(abilities);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<AbilityModel>> UpdateAsync(Guid id, UpdateAbilityPayload payload, CancellationToken cancellationToken)
  {
    AbilityModel? ability = await _abilityService.UpdateAsync(id, payload, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  private ActionResult<AbilityModel> ToActionResult(CreateOrReplaceAbilityResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/abilities/{result.Ability.Id}", UriKind.Absolute);
      return Created(location, result.Ability);
    }

    return Ok(result.Ability);
  }
}
