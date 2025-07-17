namespace PokeGame.Core.Items;

public interface IItemRepository
{
  Task<Item?> LoadAsync(ItemId itemId, CancellationToken cancellationToken = default);
  Task<Item?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(Item item, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Item> items, CancellationToken cancellationToken = default);
}
