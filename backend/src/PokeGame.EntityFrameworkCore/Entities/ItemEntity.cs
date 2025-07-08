using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core.Items;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.Infrastructure.Data;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class ItemEntity : AggregateEntity
{
  public int ItemId { get; private set; }
  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public int Price { get; private set; }

  public ItemCategory Category { get; private set; }

  public string? BattleItem { get; private set; }

  public MoveEntity? Move { get; private set; }
  public int? MoveId { get; private set; }
  public Guid? MoveUid { get; private set; }

  public string? Sprite { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public ItemEntity(BattleItemPublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Update(published);
  }
  public ItemEntity(ItemPublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Update(published);
  }
  public ItemEntity(TechnicalMachinePublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Update(published);
  }

  private ItemEntity() : base()
  {
  }

  public void SetMove(MoveEntity move)
  {
    if (Category != ItemCategory.TechnicalMachine)
    {
      throw new InvalidOperationException($"Cannot set the move of an item belonging to the category '{Category}'.");
    }

    Move = move;
    MoveId = move.MoveId;
    MoveUid = move.Id;
  }

  public void Update(BattleItemPublished published)
  {
    ContentLocale invariant = published.Invariant;
    ContentLocale locale = published.Locale;

    Update(published.Event);

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    Price = (int)invariant.GetNumberValue(BattleItems.Price);

    Category = ItemCategory.BattleItem;

    BattleItem = string.Join(',',
      invariant.GetNumberValue(BattleItems.AttackChange),
      invariant.GetNumberValue(BattleItems.DefenseChange),
      invariant.GetNumberValue(BattleItems.SpecialAttackChange),
      invariant.GetNumberValue(BattleItems.SpecialDefenseChange),
      invariant.GetNumberValue(BattleItems.SpeedChange),
      invariant.GetNumberValue(BattleItems.AccuracyChange),
      invariant.GetNumberValue(BattleItems.EvasionChange),
      invariant.GetNumberValue(BattleItems.CriticalChange),
      invariant.GetNumberValue(BattleItems.GuardTurns));

    Sprite = invariant.TryGetStringValue(BattleItems.Sprite);

    Url = locale.TryGetStringValue(BattleItems.Url);
    Notes = locale.TryGetStringValue(BattleItems.Notes);
  }
  public void Update(ItemPublished published)
  {
    ContentLocale invariant = published.Invariant;
    ContentLocale locale = published.Locale;

    Update(published.Event);

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    Price = (int)invariant.GetNumberValue(Items.Price);

    Category = ParseCategory(invariant.TryGetSelectValue(Items.Category)?.Single());

    Sprite = invariant.TryGetStringValue(Items.Sprite);

    Url = locale.TryGetStringValue(Items.Url);
    Notes = locale.TryGetStringValue(Items.Notes);
  }
  public void Update(TechnicalMachinePublished published)
  {
    ContentLocale invariant = published.Invariant;
    ContentLocale locale = published.Locale;

    Update(published.Event);

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    Price = (int)invariant.GetNumberValue(TechnicalMachines.Price);

    Category = ItemCategory.TechnicalMachine;

    Sprite = invariant.TryGetStringValue(TechnicalMachines.Sprite);

    Url = locale.TryGetStringValue(TechnicalMachines.Url);
    Notes = locale.TryGetStringValue(TechnicalMachines.Notes);
  }
  private static ItemCategory ParseCategory(string? value) => value switch
  {
    "tm-material" => ItemCategory.TechnicalMachineMaterial,
    "treasure" => ItemCategory.Treasure,
    "picnic" => ItemCategory.PicnicItem,
    "key" => ItemCategory.KeyItem,
    _ => ItemCategory.OtherItem,
  };

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
