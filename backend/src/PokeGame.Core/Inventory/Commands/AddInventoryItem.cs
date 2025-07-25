using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Inventory.Models;
using PokeGame.Core.Inventory.Validators;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Inventory.Commands;

internal record AddInventoryItem(Guid TrainerId, Guid ItemId, InventoryQuantityPayload Payload) : ICommand<InventoryItemModel>;

/// <exception cref="ItemNotFoundException"></exception>
/// <exception cref="TrainerNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class AddInventoryItemHandler : ICommandHandler<AddInventoryItem, InventoryItemModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IItemQuerier _itemQuerier;
  private readonly IItemRepository _itemRepository;
  private readonly ITrainerRepository _trainerRepository;

  public AddInventoryItemHandler(IApplicationContext applicationContext, IItemQuerier itemQuerier, IItemRepository itemRepository, ITrainerRepository trainerRepository)
  {
    _applicationContext = applicationContext;
    _itemQuerier = itemQuerier;
    _itemRepository = itemRepository;
    _trainerRepository = trainerRepository;
  }

  public async Task<InventoryItemModel> HandleAsync(AddInventoryItem command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    InventoryQuantityPayload payload = command.Payload;
    new InventoryQuantityValidator().ValidateAndThrow(payload);

    TrainerId trainerId = new(command.TrainerId);
    Trainer trainer = await _trainerRepository.LoadAsync(trainerId, cancellationToken)
      ?? throw new TrainerNotFoundException(command.TrainerId.ToString(), nameof(command.TrainerId));

    ItemId itemId = new(command.ItemId);
    Item item = await _itemRepository.LoadAsync(itemId, cancellationToken)
      ?? throw new ItemNotFoundException(command.ItemId.ToString(), nameof(command.ItemId));

    TrainerInventory inventory = await _trainerRepository.LoadInventoryAsync(trainer, cancellationToken);
    inventory.Add(item, payload.Quantity < 1 ? 1 : payload.Quantity, actorId);

    await _trainerRepository.SaveAsync(inventory, cancellationToken);

    ItemModel model = await _itemQuerier.ReadAsync(item, cancellationToken);
    return new InventoryItemModel(model, inventory.Quantities[item.Id]);
  }
}
