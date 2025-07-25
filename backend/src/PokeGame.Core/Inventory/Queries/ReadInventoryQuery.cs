using Krakenar.Core;
using PokeGame.Core.Inventory.Models;

namespace PokeGame.Core.Inventory.Queries;

internal record ReadInventoryQuery(Guid TrainerId) : IQuery<IReadOnlyCollection<InventoryItemModel>>;

internal class ReadInventoryQueryHandler : IQueryHandler<ReadInventoryQuery, IReadOnlyCollection<InventoryItemModel>>
{
  private readonly IInventoryQuerier _inventoryQuerier;

  public ReadInventoryQueryHandler(IInventoryQuerier inventoryQuerier)
  {
    _inventoryQuerier = inventoryQuerier;
  }

  public async Task<IReadOnlyCollection<InventoryItemModel>> HandleAsync(ReadInventoryQuery query, CancellationToken cancellationToken)
  {
    return await _inventoryQuerier.ReadAsync(query.TrainerId, cancellationToken);
  }
}
