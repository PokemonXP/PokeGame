using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Events;
using PokeGame.Core.Items.Models;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class ItemEntity : AggregateEntity
{
  public int ItemId { get; private set; }
  public Guid Id { get; private set; }

  public ItemCategory Category { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public int Price { get; private set; }

  public string? Sprite { get; private set; }
  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public MoveEntity? Move { get; private set; }
  public int? MoveId { get; private set; }
  public Guid? MoveUid { get; private set; }

  public string? Properties { get; private set; }

  public List<PokemonEntity> ContainedPokemon { get; private set; } = [];
  public List<PokemonEntity> HoldingPokemon { get; private set; } = [];
  public List<PokemonMoveEntity> LearnedMoves { get; private set; } = [];
  public List<InventoryEntity> Inventory { get; private set; } = [];

  public ItemEntity(ItemCreated @event) : base(@event)
  {
    Id = new ItemId(@event.StreamId).ToGuid();

    Category = @event.Category;

    UniqueName = @event.UniqueName.Value;

    Price = @event.Price?.Value ?? 0;
  }

  private ItemEntity() : base()
  {
  }

  public void SetProperties(BattleItemPropertiesChanged @event)
  {
    Update(@event);

    BattleItemPropertiesModel properties = new(@event.Properties);
    Properties = PokemonSerializer.Instance.Serialize(properties);
  }
  public void SetProperties(BerryPropertiesChanged @event)
  {
    Update(@event);

    BerryPropertiesModel properties = new(@event.Properties);
    Properties = PokemonSerializer.Instance.Serialize(properties);
  }
  public void SetProperties(KeyItemPropertiesChanged @event)
  {
    Update(@event);

    KeyItemPropertiesModel properties = new(@event.Properties);
    Properties = PokemonSerializer.Instance.Serialize(properties);
  }
  public void SetProperties(MaterialPropertiesChanged @event)
  {
    Update(@event);

    MaterialPropertiesModel properties = new(@event.Properties);
    Properties = PokemonSerializer.Instance.Serialize(properties);
  }
  public void SetProperties(MedicinePropertiesChanged @event)
  {
    Update(@event);

    MedicinePropertiesModel properties = new(@event.Properties);
    Properties = PokemonSerializer.Instance.Serialize(properties);
  }
  public void SetProperties(OtherItemPropertiesChanged @event)
  {
    Update(@event);

    OtherItemPropertiesModel properties = new(@event.Properties);
    Properties = PokemonSerializer.Instance.Serialize(properties);
  }
  public void SetProperties(PokeBallPropertiesChanged @event)
  {
    Update(@event);

    PokeBallPropertiesModel properties = new(@event.Properties);
    Properties = PokemonSerializer.Instance.Serialize(properties);
  }
  public void SetProperties(MoveEntity move, TechnicalMachinePropertiesChanged @event)
  {
    Update(@event);

    Move = move;
    MoveId = move.MoveId;
    MoveUid = move.Id;

    Properties = null;
  }
  public void SetProperties(TreasurePropertiesChanged @event)
  {
    Update(@event);

    TreasurePropertiesModel properties = new(@event.Properties);
    Properties = PokemonSerializer.Instance.Serialize(properties);
  }

  public void SetUniqueName(ItemUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(ItemUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Price is not null)
    {
      Price = @event.Price.Value?.Value ?? 0;
    }

    if (@event.Sprite is not null)
    {
      Sprite = @event.Sprite.Value?.Value;
    }
    if (@event.Url is not null)
    {
      Url = @event.Url.Value?.Value;
    }
    if (@event.Notes is not null)
    {
      Notes = @event.Notes.Value?.Value;
    }
  }

  public PokeBallPropertiesModel? GetPokeBallProperties() => Properties is null ? null : PokemonSerializer.Instance.Deserialize<PokeBallPropertiesModel>(Properties);

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
