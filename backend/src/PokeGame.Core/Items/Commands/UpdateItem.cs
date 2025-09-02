using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Items.Validators;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Items.Commands;

internal record UpdateItem(Guid Id, UpdateItemPayload Payload) : ICommand<ItemModel?>;

/// <exception cref="MoveNotFoundException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateItemHandler : ICommandHandler<UpdateItem, ItemModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IItemManager _itemManager;
  private readonly IItemQuerier _itemQuerier;
  private readonly IItemRepository _itemRepository;
  private readonly IMoveRepository _moveRepository;

  public UpdateItemHandler(
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

  public async Task<ItemModel?> HandleAsync(UpdateItem command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdateItemPayload payload = command.Payload;
    new UpdateItemValidator(uniqueNameSettings).ValidateAndThrow(payload);

    ItemId itemId = new(command.Id);
    Item? item = await _itemRepository.LoadAsync(itemId, cancellationToken);
    if (item is null)
    {
      return null;
    }
    new UpdateItemValidator(item.Category).ValidateAndThrow(payload);

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      item.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      item.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      item.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Price.HasValue)
    {
      item.Price = payload.Price.Value < 1 ? null : new Price(payload.Price.Value);
    }

    ItemProperties? properties = await GetPropertiesAsync(payload, cancellationToken);
    if (properties is not null)
    {
      item.SetProperties(properties, actorId);
    }

    if (payload.Sprite is not null)
    {
      item.Sprite = Url.TryCreate(payload.Sprite.Value);
    }
    if (payload.Url is not null)
    {
      item.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      item.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    item.Update(_applicationContext.ActorId);
    await _itemManager.SaveAsync(item, cancellationToken);

    return await _itemQuerier.ReadAsync(item, cancellationToken);
  }

  private async Task<ItemProperties?> GetPropertiesAsync(UpdateItemPayload payload, CancellationToken cancellationToken)
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
    return properties.SingleOrDefault();
  }
}
