using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Item;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("items")]
public class ItemController : ControllerBase
{
  private readonly IItemQuerier _itemQuerier;

  public ItemController(IItemQuerier itemQuerier)
  {
    _itemQuerier = itemQuerier;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ItemModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ItemModel? item = await _itemQuerier.ReadAsync(id, cancellationToken);
    return item is null ? NotFound() : Ok(item);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<ItemModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ItemModel? item = await _itemQuerier.ReadAsync(uniqueName, cancellationToken);
    return item is null ? NotFound() : Ok(item);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ItemModel>>> SearchAsync([FromQuery] SearchItemsParameters parameters, CancellationToken cancellationToken)
  {
    SearchItemsPayload payload = parameters.ToPayload();
    SearchResults<ItemModel> items = await _itemQuerier.SearchAsync(payload, cancellationToken);
    return Ok(items);
  }
}
