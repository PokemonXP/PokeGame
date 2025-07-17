using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Variety;
using PokeGame.Core.Varieties;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("varieties")]
public class VarietyController : ControllerBase
{
  private readonly IVarietyService _varietyService;

  public VarietyController(IVarietyService varietyService)
  {
    _varietyService = varietyService;
  }

  [HttpPost]
  public async Task<ActionResult<VarietyModel>> CreateAsync([FromBody] CreateOrReplaceVarietyPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceVarietyResult result = await _varietyService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<VarietyModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    VarietyModel? variety = await _varietyService.DeleteAsync(id, cancellationToken);
    return variety is null ? NotFound() : Ok(variety);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<VarietyModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    VarietyModel? variety = await _varietyService.ReadAsync(id, uniqueName: null, cancellationToken);
    return variety is null ? NotFound() : Ok(variety);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<VarietyModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    VarietyModel? variety = await _varietyService.ReadAsync(id: null, uniqueName, cancellationToken);
    return variety is null ? NotFound() : Ok(variety);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<VarietyModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceVarietyPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceVarietyResult result = await _varietyService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<VarietyModel>>> SearchAsync([FromQuery] SearchVarietiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchVarietiesPayload payload = parameters.ToPayload();
    SearchResults<VarietyModel> varieties = await _varietyService.SearchAsync(payload, cancellationToken);
    return Ok(varieties);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<VarietyModel>> UpdateAsync(Guid id, [FromBody] UpdateVarietyPayload payload, CancellationToken cancellationToken)
  {
    VarietyModel? variety = await _varietyService.UpdateAsync(id, payload, cancellationToken);
    return variety is null ? NotFound() : Ok(variety);
  }

  private ActionResult<VarietyModel> ToActionResult(CreateOrReplaceVarietyResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/varieties/{result.Variety.Id}", UriKind.Absolute);
      return Created(location, result.Variety);
    }

    return Ok(result.Variety);
  }
}
