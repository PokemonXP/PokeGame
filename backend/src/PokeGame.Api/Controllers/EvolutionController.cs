using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Evolution;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Evolutions.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("evolutions")]
public class EvolutionController : ControllerBase
{
  private readonly IEvolutionService _evolutionService;

  public EvolutionController(IEvolutionService evolutionService)
  {
    _evolutionService = evolutionService;
  }

  [HttpPost]
  public async Task<ActionResult<EvolutionModel>> CreateAsync([FromBody] CreateOrReplaceEvolutionPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceEvolutionResult result = await _evolutionService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<EvolutionModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    EvolutionModel? evolution = await _evolutionService.DeleteAsync(id, cancellationToken);
    return evolution is null ? NotFound() : Ok(evolution);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<EvolutionModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    EvolutionModel? evolution = await _evolutionService.ReadAsync(id, cancellationToken);
    return evolution is null ? NotFound() : Ok(evolution);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<EvolutionModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceEvolutionPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceEvolutionResult result = await _evolutionService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<EvolutionModel>>> SearchAsync([FromQuery] SearchEvolutionsParameters parameters, CancellationToken cancellationToken)
  {
    SearchEvolutionsPayload payload = parameters.ToPayload();
    SearchResults<EvolutionModel> evolutions = await _evolutionService.SearchAsync(payload, cancellationToken);
    return Ok(evolutions);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<EvolutionModel>> UpdateAsync(Guid id, [FromBody] UpdateEvolutionPayload payload, CancellationToken cancellationToken)
  {
    EvolutionModel? evolution = await _evolutionService.UpdateAsync(id, payload, cancellationToken);
    return evolution is null ? NotFound() : Ok(evolution);
  }

  private ActionResult<EvolutionModel> ToActionResult(CreateOrReplaceEvolutionResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/evolutions/{result.Evolution.Id}", UriKind.Absolute);
      return Created(location, result.Evolution);
    }

    return Ok(result.Evolution);
  }
}
