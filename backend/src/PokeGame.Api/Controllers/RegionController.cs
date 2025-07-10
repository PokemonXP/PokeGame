using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Region;
using PokeGame.Core.Regions;
using PokeGame.Core.Regions.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("api/regions")]
public class RegionController : ControllerBase
{
  private readonly IRegionQuerier _regionQuerier;

  public RegionController(IRegionQuerier regionQuerier)
  {
    _regionQuerier = regionQuerier;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<RegionModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RegionModel? region = await _regionQuerier.ReadAsync(id, cancellationToken);
    return region is null ? NotFound() : Ok(region);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<RegionModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    RegionModel? region = await _regionQuerier.ReadAsync(uniqueName, cancellationToken);
    return region is null ? NotFound() : Ok(region);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<RegionModel>>> SearchAsync([FromQuery] SearchRegionsParameters parameters, CancellationToken cancellationToken)
  {
    SearchRegionsPayload payload = parameters.ToPayload();
    SearchResults<RegionModel> regions = await _regionQuerier.SearchAsync(payload, cancellationToken);
    return Ok(regions);
  }
}
