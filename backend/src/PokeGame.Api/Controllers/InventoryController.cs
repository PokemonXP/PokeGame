﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Core.Inventory;
using PokeGame.Core.Inventory.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("trainers/{trainerId}/inventory")]
public class InventoryController : ControllerBase
{
  private readonly IInventoryService _inventoryService;

  public InventoryController(IInventoryService inventoryService)
  {
    _inventoryService = inventoryService;
  }

  [HttpPost("{itemId}")]
  public async Task<ActionResult<InventoryItemModel>> AddAsync(Guid trainerId, Guid itemId, [FromBody] InventoryQuantityPayload payload, CancellationToken cancellationToken)
  {
    InventoryItemModel item = await _inventoryService.AddAsync(trainerId, itemId, payload, cancellationToken);
    return Ok(item);
  }

  [HttpGet]
  public async Task<ActionResult<IReadOnlyCollection<InventoryItemModel>>> ReadAsync(Guid trainerId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<InventoryItemModel> inventory = await _inventoryService.ReadAsync(trainerId, cancellationToken);
    return Ok(inventory);
  }

  [HttpGet("{itemId}")]
  public async Task<ActionResult<InventoryItemModel>> ReadAsync(Guid trainerId, Guid itemId, CancellationToken cancellationToken)
  {
    InventoryItemModel? inventory = await _inventoryService.ReadAsync(trainerId, itemId, cancellationToken);
    return inventory is null ? NotFound() : Ok(inventory);
  }

  [HttpDelete("{itemId}")]
  public async Task<ActionResult<InventoryItemModel>> RemoveAsync(Guid trainerId, Guid itemId, int? quantity, CancellationToken cancellationToken)
  {
    InventoryQuantityPayload? payload = quantity.HasValue ? new(quantity.Value) : null;
    InventoryItemModel item = await _inventoryService.RemoveAsync(trainerId, itemId, payload, cancellationToken);
    return Ok(item);
  }

  [HttpPut("{itemId}")]
  public async Task<ActionResult<InventoryItemModel>> UpdateAsync(Guid trainerId, Guid itemId, [FromBody] InventoryQuantityPayload payload, CancellationToken cancellationToken)
  {
    InventoryItemModel item = await _inventoryService.UpdateAsync(trainerId, itemId, payload, cancellationToken);
    return Ok(item);
  }
}
