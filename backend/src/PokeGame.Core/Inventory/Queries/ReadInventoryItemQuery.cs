using Krakenar.Core;
using PokeGame.Core.Inventory.Models;

namespace PokeGame.Core.Inventory.Queries;

internal record ReadInventoryItemQuery(Guid TrainerId, Guid ItemId) : IQuery<InventoryItemModel?>;

internal class ReadInventoryItemQueryHandler : IQueryHandler<ReadInventoryItemQuery, InventoryItemModel?>
{
  private readonly IInventoryQuerier _inventoryQuerier;

  public ReadInventoryItemQueryHandler(IInventoryQuerier inventoryQuerier)
  {
    _inventoryQuerier = inventoryQuerier;
  }

  public async Task<InventoryItemModel?> HandleAsync(ReadInventoryItemQuery query, CancellationToken cancellationToken)
  {
    return await _inventoryQuerier.ReadAsync(query.TrainerId, query.ItemId, cancellationToken);
  }
}
