using Krakenar.Core;
using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items.Commands;

internal record DeleteItem(Guid Id) : ICommand<ItemModel?>;

internal class DeleteItemHandler : ICommandHandler<DeleteItem, ItemModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IItemQuerier _itemQuerier;
  private readonly IItemRepository _itemRepository;

  public DeleteItemHandler(IApplicationContext applicationContext, IItemQuerier itemQuerier, IItemRepository itemRepository)
  {
    _applicationContext = applicationContext;
    _itemQuerier = itemQuerier;
    _itemRepository = itemRepository;
  }

  public async Task<ItemModel?> HandleAsync(DeleteItem command, CancellationToken cancellationToken)
  {
    ItemId itemId = new(command.Id);
    Item? item = await _itemRepository.LoadAsync(itemId, cancellationToken);
    if (item is null)
    {
      return null;
    }
    ItemModel model = await _itemQuerier.ReadAsync(item, cancellationToken);

    item.Delete(_applicationContext.ActorId);
    await _itemRepository.SaveAsync(item, cancellationToken);

    return model;
  }
}
