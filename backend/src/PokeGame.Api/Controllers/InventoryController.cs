using Microsoft.AspNetCore.Authorization;
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
  public async Task<ActionResult> AddAsync(Guid trainerId, Guid itemId, [FromBody] AddInventoryItemPayload payload, CancellationToken cancellationToken)
  {
    InventoryItemModel item = await _inventoryService.AddAsync(trainerId, itemId, payload, cancellationToken);
    return Ok(item);
  }
}
