using PokeGame.Core.Items.Events;

namespace PokeGame.Core.Items;

internal interface IItemManager
{
  Task SaveAsync(Item item, CancellationToken cancellationToken = default);
}

internal class ItemManager : IItemManager
{
  private readonly IItemQuerier _itemQuerier;
  private readonly IItemRepository _itemRepository;

  public ItemManager(IItemQuerier itemQuerier, IItemRepository itemRepository)
  {
    _itemQuerier = itemQuerier;
    _itemRepository = itemRepository;
  }

  public async Task SaveAsync(Item item, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = item.Changes.Any(change => change is ItemCreated || change is ItemUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      ItemId? conflictId = await _itemQuerier.FindIdAsync(item.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(item.Id))
      {
        throw new UniqueNameAlreadyUsedException(item, conflictId.Value);
      }
    }

    await _itemRepository.SaveAsync(item, cancellationToken);
  }
}
