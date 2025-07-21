using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Items.Events;
using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items;

public class Item : AggregateRoot
{
  private ItemUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null || _updated.Description is not null
    || _updated.Price is not null
    || _updated.Sprite is not null || _updated.Url is not null || _updated.Notes is not null;

  public new ItemId Id => new(base.Id);

  public ItemCategory Category { get; private set; }

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The item has not been initialized.");
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

  private Price? _price = null;
  public Price? Price
  {
    get => _price;
    set
    {
      if (_price != value)
      {
        _price = value;
        _updated.Price = new Change<Price>(value);
      }
    }
  }

  private ItemProperties? _properties = null;
  public ItemProperties Properties => _properties ?? throw new InvalidOperationException("The item has not been initialized.");

  private Url? _sprite = null;
  public Url? Sprite
  {
    get => _sprite;
    set
    {
      if (_sprite != value)
      {
        _sprite = value;
        _updated.Sprite = new Change<Url>(value);
      }
    }
  }
  private Url? _url = null;
  public Url? Url
  {
    get => _url;
    set
    {
      if (_url != value)
      {
        _url = value;
        _updated.Url = new Change<Url>(value);
      }
    }
  }
  private Notes? _notes = null;
  public Notes? Notes
  {
    get => _notes;
    set
    {
      if (_notes != value)
      {
        _notes = value;
        _updated.Notes = new Change<Notes>(value);
      }
    }
  }

  public Item() : base()
  {
  }

  public Item(UniqueName uniqueName, ItemProperties properties, Price? price = null, ActorId? actorId = null, ItemId? itemId = null)
    : base((itemId ?? ItemId.NewId()).StreamId)
  {
    if (!Enum.IsDefined(properties.Category))
    {
      throw new ArgumentOutOfRangeException(nameof(properties), "The item category is not defined.");
    }

    Raise(new ItemCreated(properties.Category, uniqueName, price), actorId);

    SetProperties(properties, actorId);
  }
  protected virtual void Handle(ItemCreated @event)
  {
    Category = @event.Category;

    _uniqueName = @event.UniqueName;

    _price = @event.Price;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ItemDeleted(), actorId);
    }
  }

  public void SetProperties(ItemProperties properties, ActorId? actorId = null)
  {
    if (Category != properties.Category)
    {
      throw new ArgumentException($"Cannot set properties of category '{properties.Category}' on an item in category '{Category}'.", nameof(properties));
    }

    switch (properties.Category)
    {
      case ItemCategory.BattleItem:
        Raise(new BattleItemPropertiesChanged((BattleItemProperties)properties), actorId);
        break;
      case ItemCategory.Berry:
        Raise(new BerryPropertiesChanged((BerryProperties)properties), actorId);
        break;
      case ItemCategory.KeyItem:
        Raise(new KeyItemPropertiesChanged((KeyItemProperties)properties), actorId);
        break;
      case ItemCategory.Material:
        Raise(new MaterialPropertiesChanged((MaterialProperties)properties), actorId);
        break;
      case ItemCategory.Medicine:
        Raise(new MedicinePropertiesChanged((MedicineProperties)properties), actorId);
        break;
      case ItemCategory.OtherItem:
        Raise(new OtherItemPropertiesChanged((OtherItemProperties)properties), actorId);
        break;
      case ItemCategory.PokeBall:
        Raise(new PokeBallPropertiesChanged((PokeBallProperties)properties), actorId);
        break;
      case ItemCategory.TechnicalMachine:
        Raise(new TechnicalMachinePropertiesChanged((TechnicalMachineProperties)properties), actorId);
        break;
      case ItemCategory.Treasure:
        Raise(new TreasurePropertiesChanged((TreasureProperties)properties), actorId);
        break;
      default:
        throw new ItemCategoryNotSupportedException(properties.Category);
    }
  }
  protected virtual void Handle(BattleItemPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }
  protected virtual void Handle(BerryPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }
  protected virtual void Handle(KeyItemPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }
  protected virtual void Handle(MaterialPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }
  protected virtual void Handle(MedicinePropertiesChanged @event)
  {
    _properties = @event.Properties;
  }
  protected virtual void Handle(OtherItemPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }
  protected virtual void Handle(PokeBallPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }
  protected virtual void Handle(TechnicalMachinePropertiesChanged @event)
  {
    _properties = @event.Properties;
  }
  protected virtual void Handle(TreasurePropertiesChanged @event)
  {
    _properties = @event.Properties;
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new ItemUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(ItemUniqueNameChanged @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(ItemUpdated @event)
  {
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Price is not null)
    {
      _price = @event.Price.Value;
    }

    if (@event.Sprite is not null)
    {
      _sprite = @event.Sprite.Value;
    }
    if (@event.Url is not null)
    {
      _url = @event.Url.Value;
    }
    if (@event.Notes is not null)
    {
      _notes = @event.Notes.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
