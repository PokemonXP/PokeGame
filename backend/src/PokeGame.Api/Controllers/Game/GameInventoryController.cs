using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Extensions;
using PokeGame.Api.Models.Game;
using PokeGame.Core.Inventory;
using PokeGame.Core.Inventory.Models;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Api.Controllers.Game;

[ApiController]
[Authorize]
[Route("game/trainers/{trainerId}/inventory")]
public class GameInventoryController : ControllerBase
{
  private readonly IInventoryService _inventoryService;
  private readonly ITrainerService _trainerService;

  public GameInventoryController(IInventoryService inventoryService, ITrainerService trainerService)
  {
    _inventoryService = inventoryService;
    _trainerService = trainerService;
  }

  [HttpGet]
  public async Task<ActionResult<Inventory>> GetAsync(Guid trainerId, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerService.ReadAsync(trainerId, uniqueName: null, license: null, cancellationToken);
    if (trainer is null)
    {
      return NotFound();
    }
    else if (trainer.UserId != HttpContext.GetUserId())
    {
      return Forbid();
    }

    IReadOnlyCollection<InventoryItemModel> items = await _inventoryService.ReadAsync(trainer.Id, cancellationToken);

    Inventory inventory = new(trainer, items);
    return Ok(inventory);
  }
}
