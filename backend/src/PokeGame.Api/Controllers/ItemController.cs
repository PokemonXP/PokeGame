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
  private readonly IItemService _itemService;

  public ItemController(IItemService itemService)
  {
    _itemService = itemService;
  }

  [HttpPost]
  public async Task<ActionResult<ItemModel>> CreateAsync([FromBody] CreateOrReplaceItemPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<ItemModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    ItemModel? item = await _itemService.DeleteAsync(id, cancellationToken);
    return item is null ? NotFound() : Ok(item);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ItemModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ItemModel? item = await _itemService.ReadAsync(id, uniqueName: null, cancellationToken);
    return item is null ? NotFound() : Ok(item);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<ItemModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ItemModel? item = await _itemService.ReadAsync(id: null, uniqueName, cancellationToken);
    return item is null ? NotFound() : Ok(item);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ItemModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceItemPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ItemModel>>> SearchAsync([FromQuery] SearchItemsParameters parameters, CancellationToken cancellationToken)
  {
    SearchItemsPayload payload = parameters.ToPayload();
    SearchResults<ItemModel> items = await _itemService.SearchAsync(payload, cancellationToken);
    return Ok(items);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ItemModel>> UpdateAsync(Guid id, [FromBody] UpdateItemPayload payload, CancellationToken cancellationToken)
  {
    ItemModel? item = await _itemService.UpdateAsync(id, payload, cancellationToken);
    return item is null ? NotFound() : Ok(item);
  }

  private ActionResult<ItemModel> ToActionResult(CreateOrReplaceItemResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/items/{result.Item.Id}", UriKind.Absolute);
      return Created(location, result.Item);
    }

    return Ok(result.Item);
  }
}
