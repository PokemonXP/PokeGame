using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Cms.Models.Species;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Models;

namespace PokeGame.Cms.Controllers;

[ApiController]
[Route("api/species")]
public class SpeciesController : ControllerBase
{
  private readonly ISpeciesQuerier _speciesQuerier;

  public SpeciesController(ISpeciesQuerier speciesQuerier)
  {
    _speciesQuerier = speciesQuerier;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<SpeciesModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SpeciesModel? species = await _speciesQuerier.ReadAsync(id, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<SpeciesModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    SpeciesModel? species = await _speciesQuerier.ReadAsync(uniqueName, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet("number:{number}")]
  public async Task<ActionResult<SpeciesModel>> ReadAsync(int number, [FromQuery(Name = "region")] Guid? regionId, CancellationToken cancellationToken)
  {
    SpeciesModel? species = await _speciesQuerier.ReadAsync(number, regionId, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<SpeciesModel>>> SearchAsync([FromQuery] SearchSpeciesParameters parameters, CancellationToken cancellationToken)
  {
    SearchSpeciesPayload payload = parameters.ToPayload();
    SearchResults<SpeciesModel> species = await _speciesQuerier.SearchAsync(payload, cancellationToken);
    return Ok(species);
  }
}
