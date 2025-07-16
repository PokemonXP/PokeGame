using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items;

internal interface IItemManager
{
  Task<Item> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken = default);
}

internal class ItemManager : IItemManager
{
  private readonly IItemQuerier _itemQuerier;

  public ItemManager(IItemQuerier itemQuerier)
  {
    _itemQuerier = itemQuerier;
  }

  public async Task<Item> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken)
  {
    ItemModel? item = null;
    if (Guid.TryParse(idOrUniqueName, out Guid itemId))
    {
      item = await _itemQuerier.ReadAsync(itemId, cancellationToken);
    }
    item ??= await _itemQuerier.ReadAsync(idOrUniqueName, cancellationToken) ?? throw new ItemNotFoundException(idOrUniqueName, propertyName);
    return item.ToItem();
  }
}
