using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items;

internal interface IItemManager
{
  Task<ItemModel> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken = default);
}

internal class ItemManager : IItemManager
{
  private readonly IItemQuerier _itemQuerier;

  public ItemManager(IItemQuerier itemQuerier)
  {
    _itemQuerier = itemQuerier;
  }

  public async Task<ItemModel> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken)
  {
    ItemModel? item = null;
    if (Guid.TryParse(idOrUniqueName, out Guid itemId))
    {
      item = await _itemQuerier.ReadAsync(itemId, cancellationToken);
    }
    return item ?? await _itemQuerier.ReadAsync(idOrUniqueName, cancellationToken) ?? throw new ItemNotFoundException(idOrUniqueName, propertyName);
  }
}
