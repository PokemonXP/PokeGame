using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Items;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class ItemRepository : Repository, IItemRepository
{
  private readonly DbSet<ItemEntity> _items;

  public ItemRepository(PokemonContext context, IEventStore eventStore) : base(eventStore)
  {
    _items = context.Items;
  }

  public async Task<Item?> LoadAsync(ItemId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Item>(id.StreamId, cancellationToken);
  }

  public async Task<Item?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    ItemId itemId;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      itemId = new(id);
      Item? item = await LoadAsync(itemId, cancellationToken);
      if (item is not null)
      {
        return item;
      }
    }

    string uniqueNameNormalized = Helper.Normalize(idOrUniqueName);
    string? streamId = await _items.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(streamId))
    {
      return null;
    }

    itemId = new(streamId);
    return await LoadAsync(itemId, cancellationToken);
  }

  public async Task SaveAsync(Item item, CancellationToken cancellationToken)
  {
    await base.SaveAsync(item, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Item> items, CancellationToken cancellationToken)
  {
    await base.SaveAsync(items, cancellationToken);
  }
}
