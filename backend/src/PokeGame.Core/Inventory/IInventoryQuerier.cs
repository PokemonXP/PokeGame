using PokeGame.Core.Inventory.Models;

namespace PokeGame.Core.Inventory;

public interface IInventoryQuerier
{
  Task<InventoryItemModel?> ReadAsync(Guid trainerId, Guid itemId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<InventoryItemModel>> ReadAsync(Guid trainerId, CancellationToken cancellationToken = default);
}
