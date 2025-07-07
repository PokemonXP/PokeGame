using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Cms.Models.Variety;
using PokeGame.Core.Varieties;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Cms.Controllers;

[ApiController]
[Route("api/varieties")]
public class VarietyController : ControllerBase
{
  private readonly IVarietyQuerier _varietyQuerier;

  public VarietyController(IVarietyQuerier varietyQuerier)
  {
    _varietyQuerier = varietyQuerier;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<VarietyModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    VarietyModel? variety = await _varietyQuerier.ReadAsync(id, cancellationToken);
    return variety is null ? NotFound() : Ok(variety);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<VarietyModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    VarietyModel? variety = await _varietyQuerier.ReadAsync(uniqueName, cancellationToken);
    return variety is null ? NotFound() : Ok(variety);
  }

  [HttpGet("/api/species/{speciesId}/varieties")]
  public async Task<ActionResult<SearchResults<VarietyModel>>> SearchAsync(Guid speciesId, [FromQuery] SearchVarietiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchVarietiesPayload payload = parameters.ToPayload();
    SearchResults<VarietyModel> varieties = await _varietyQuerier.SearchAsync(speciesId, payload, cancellationToken);
    return Ok(varieties);
  }
}
