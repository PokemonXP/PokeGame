using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Items.Validators;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Items.Commands;

internal record CreateOrReplaceItem(CreateOrReplaceItemPayload Payload, Guid? Id) : ICommand<CreateOrReplaceItemResult>;

/// <exception cref="MoveNotFoundException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceItemHandler : ICommandHandler<CreateOrReplaceItem, CreateOrReplaceItemResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IItemManager _itemManager;
  private readonly IItemQuerier _itemQuerier;
  private readonly IItemRepository _itemRepository;
  private readonly IMoveRepository _moveRepository;

  public CreateOrReplaceItemHandler(
    IApplicationContext applicationContext,
    IItemManager itemManager,
    IItemQuerier itemQuerier,
    IItemRepository itemRepository,
    IMoveRepository moveRepository)
  {
    _applicationContext = applicationContext;
    _itemManager = itemManager;
    _itemQuerier = itemQuerier;
    _itemRepository = itemRepository;
    _moveRepository = moveRepository;
  }

  public async Task<CreateOrReplaceItemResult> HandleAsync(CreateOrReplaceItem command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreateOrReplaceItemPayload payload = command.Payload;
    new CreateOrReplaceItemValidator(uniqueNameSettings).ValidateAndThrow(payload);

    ItemId itemId = ItemId.NewId();
    Item? item = null;
    if (command.Id.HasValue)
    {
      itemId = new(command.Id.Value);
      item = await _itemRepository.LoadAsync(itemId, cancellationToken);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    ItemProperties properties = await GetPropertiesAsync(payload, cancellationToken);
    Price? price = payload.Price < 1 ? null : new(payload.Price);

    bool created = false;
    if (item is null)
    {
      item = new(uniqueName, properties, price, actorId, itemId);
      created = true;
    }
    else
    {
      item.SetUniqueName(uniqueName, actorId);

      item.Price = price;

      item.SetProperties(properties, actorId);
    }

    item.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    item.Description = Description.TryCreate(payload.Description);

    item.Sprite = Url.TryCreate(payload.Sprite);
    item.Url = Url.TryCreate(payload.Url);
    item.Notes = Notes.TryCreate(payload.Notes);

    item.Update(actorId);
    await _itemManager.SaveAsync(item, cancellationToken);

    ItemModel model = await _itemQuerier.ReadAsync(item, cancellationToken);
    return new CreateOrReplaceItemResult(model, created);
  }

  private async Task<ItemProperties> GetPropertiesAsync(CreateOrReplaceItemPayload payload, CancellationToken cancellationToken)
  {
    List<ItemProperties> properties = new(capacity: 9);

    if (payload.BattleItem is not null)
    {
      properties.Add(new BattleItemProperties(payload.BattleItem));
    }
    if (payload.Berry is not null)
    {
      properties.Add(new BerryProperties(payload.Berry));
    }
    if (payload.KeyItem is not null)
    {
      properties.Add(new KeyItemProperties(payload.KeyItem));
    }
    if (payload.Material is not null)
    {
      properties.Add(new MaterialProperties(payload.Material));
    }
    if (payload.Medicine is not null)
    {
      properties.Add(new MedicineProperties(payload.Medicine));
    }
    if (payload.OtherItem is not null)
    {
      properties.Add(new OtherItemProperties(payload.OtherItem));
    }
    if (payload.PokeBall is not null)
    {
      properties.Add(new PokeBallProperties(payload.PokeBall));
    }
    if (payload.TechnicalMachine is not null)
    {
      Move move = await _moveRepository.LoadAsync(payload.TechnicalMachine.Move, cancellationToken)
        ?? throw new MoveNotFoundException(payload.TechnicalMachine.Move, string.Join('.', nameof(payload.TechnicalMachine), nameof(payload.TechnicalMachine.Move)));
      properties.Add(new TechnicalMachineProperties(move));
    }
    if (payload.Treasure is not null)
    {
      properties.Add(new TreasureProperties(payload.Treasure));
    }

    if (properties.Count > 1)
    {
      throw new ArgumentException("Many properties were provided, exactly one is expected.", nameof(payload));
    }
    return properties.SingleOrDefault() ?? throw new ArgumentException("No property was provided, exactly one is expected.", nameof(payload));
  }
}
