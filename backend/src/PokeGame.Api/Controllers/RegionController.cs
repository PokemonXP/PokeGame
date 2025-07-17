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
[Route("regions")]
public class RegionController : ControllerBase
{
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionService _regionService;

  public RegionController(IRegionQuerier regionQuerier, IRegionService regionService)
  {
    _regionQuerier = regionQuerier;
    _regionService = regionService;
  }

  [HttpPost]
  public async Task<ActionResult<RegionModel>> CreateAsync(CreateOrReplaceRegionPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRegionResult result = await _regionService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
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

  [HttpPut("{id}")]
  public async Task<ActionResult<RegionModel>> ReplaceAsync(Guid id, CreateOrReplaceRegionPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRegionResult result = await _regionService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<RegionModel>>> SearchAsync([FromQuery] SearchRegionsParameters parameters, CancellationToken cancellationToken)
  {
    SearchRegionsPayload payload = parameters.ToPayload();
    SearchResults<RegionModel> regions = await _regionQuerier.SearchAsync(payload, cancellationToken);
    return Ok(regions);
  }

  private ActionResult<RegionModel> ToActionResult(CreateOrReplaceRegionResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/regions/{result.Region.Id}", UriKind.Absolute);
      return Created(location, result.Region);
    }

    return Ok(result.Region);
  }
}
