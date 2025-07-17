using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Species;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("species")]
public class SpeciesController : ControllerBase
{
  private readonly ISpeciesService _speciesService;

  public SpeciesController(ISpeciesService speciesService)
  {
    _speciesService = speciesService;
  }

  [HttpPost]
  public async Task<ActionResult<SpeciesModel>> CreateAsync([FromBody] CreateOrReplaceSpeciesPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpeciesResult result = await _speciesService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<SpeciesModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    SpeciesModel? species = await _speciesService.DeleteAsync(id, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<SpeciesModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SpeciesModel? species = await _speciesService.ReadAsync(id, number: null, uniqueName: null, regionId: null, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<SpeciesModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    SpeciesModel? species = await _speciesService.ReadAsync(id: null, number: null, uniqueName, regionId: null, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet("number:{number}")]
  public async Task<ActionResult<SpeciesModel>> ReadAsync(int number, [FromQuery(Name = "region")] Guid? regionId, CancellationToken cancellationToken)
  {
    SpeciesModel? species = await _speciesService.ReadAsync(id: null, number, uniqueName: null, regionId, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<SpeciesModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceSpeciesPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpeciesResult result = await _speciesService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<SpeciesModel>>> SearchAsync([FromQuery] SearchSpeciesParameters parameters, CancellationToken cancellationToken)
  {
    SearchSpeciesPayload payload = parameters.ToPayload();
    SearchResults<SpeciesModel> species = await _speciesService.SearchAsync(payload, cancellationToken);
    return Ok(species);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<SpeciesModel>> UpdateAsync(Guid id, [FromBody] UpdateSpeciesPayload payload, CancellationToken cancellationToken)
  {
    SpeciesModel? species = await _speciesService.UpdateAsync(id, payload, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  private ActionResult<SpeciesModel> ToActionResult(CreateOrReplaceSpeciesResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/species/{result.Species.Id}", UriKind.Absolute);
      return Created(location, result.Species);
    }

    return Ok(result.Species);
  }
}
