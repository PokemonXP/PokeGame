using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using PokeGame.Core;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Models;
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
  public string? Berry { get; private set; }
  public string? Medicine { get; private set; }
  public string? PokeBall { get; private set; }

  public MoveEntity? Move { get; private set; }
  public int? MoveId { get; private set; }
  public Guid? MoveUid { get; private set; }

  public string? Sprite { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public ItemEntity(BattleItemPublished published) : this(published.Event)
  {
    Update(published);
  }
  public ItemEntity(BerryPublished published) : this(published.Event)
  {
    Update(published);
  }
  public ItemEntity(ItemPublished published) : this(published.Event)
  {
    Update(published);
  }
  public ItemEntity(MedicinePublished published) : this(published.Event)
  {
    Update(published);
  }
  public ItemEntity(PokeBallPublished published) : this(published.Event)
  {
    Update(published);
  }
  public ItemEntity(TechnicalMachinePublished published) : this(published.Event)
  {
    Update(published);
  }
  private ItemEntity(DomainEvent @event) : base(@event)
  {
    Id = new ContentId(@event.StreamId).EntityId;
  }

  private ItemEntity() : base()
  {
  }

  public BattleItemModel? GetBattleItem()
  {
    int[]? values = BattleItem?.Split(',').Select(int.Parse).ToArray();
    return values is null
      ? null
      : new BattleItemModel(new StatisticChangesModel(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7]), values[8]);
  }
  public BerryModel? GetBerry() => Berry is null ? null : JsonSerializer.Deserialize<BerryModel>(Berry); // TODO(fpion): use custom serializer
  public MedicineModel? GetMedicine() => Medicine is null ? null : JsonSerializer.Deserialize<MedicineModel>(Medicine); // TODO(fpion): use custom serializer
  public PokeBallModel? GetPokeBall() => PokeBall is null ? null : JsonSerializer.Deserialize<PokeBallModel>(PokeBall); // TODO(fpion): use custom serializer

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
    Update(published.Event, published.Invariant, published.Locale, BattleItems.Price, ItemCategory.BattleItem, BattleItems.Sprite, BattleItems.Url, BattleItems.Notes);

    ContentLocale invariant = published.Invariant;
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
  }
  public void Update(BerryPublished published)
  {
    Update(published.Event, published.Invariant, published.Locale, Berries.Price, ItemCategory.Berry, Berries.Sprite, Berries.Url, Berries.Notes);

    ContentLocale invariant = published.Invariant;

    BerryModel berry = new()
    {
      Healing = (int)invariant.GetNumberValue(Berries.Healing),
      IsHealingPercentage = invariant.GetBooleanValue(Berries.IsHealingPercentage),
      CureConfusion = invariant.GetBooleanValue(Berries.CureConfusion),
      CureNonVolatileConditions = invariant.GetBooleanValue(Berries.CureNonVolatileConditions),
      PowerPoints = (int)invariant.GetNumberValue(Berries.PowerPoints),
      StatisticChanges = new StatisticChangesModel(
        (int)invariant.GetNumberValue(Berries.AttackChange),
        (int)invariant.GetNumberValue(Berries.DefenseChange),
        (int)invariant.GetNumberValue(Berries.SpecialAttackChange),
        (int)invariant.GetNumberValue(Berries.SpecialDefenseChange),
        (int)invariant.GetNumberValue(Berries.SpeedChange),
        (int)invariant.GetNumberValue(Berries.AccuracyChange),
        (int)invariant.GetNumberValue(Berries.EvasionChange),
        (int)invariant.GetNumberValue(Berries.CriticalChange)),
      RaiseFriendship = invariant.GetBooleanValue(Berries.RaiseFriendship)
    };

    IReadOnlyCollection<string> statusConditions = invariant.GetSelectValue(Berries.StatusCondition);
    berry.StatusCondition = statusConditions.Count < 1 ? null : PokemonConverter.Instance.ToStatusCondition(statusConditions.Single());

    IReadOnlyCollection<string> lowerEffortValues = invariant.GetSelectValue(Berries.LowerEffortValues);
    berry.LowerEffortValues = lowerEffortValues.Count < 1 ? null : PokemonConverter.Instance.ToStatistic(lowerEffortValues.Single());

    Berry = JsonSerializer.Serialize(berry); // TODO(fpion): use custom serializer
  }
  public void Update(ItemPublished published)
  {
    ContentLocale invariant = published.Invariant;

    IReadOnlyCollection<string> categories = invariant.TryGetSelectValue(Items.Category) ?? [];
    ItemCategory category = categories.Count < 1 ? ItemCategory.OtherItem : PokemonConverter.Instance.ToItemCategory(categories.Single());

    Update(published.Event, published.Invariant, published.Locale, Items.Price, category, Items.Sprite, Items.Url, Items.Notes);
  }
  public void Update(MedicinePublished published)
  {
    Update(published.Event, published.Invariant, published.Locale, Medicines.Price, ItemCategory.Medicine, Medicines.Sprite, Medicines.Url, Medicines.Notes);

    ContentLocale invariant = published.Invariant;

    MedicineModel medicine = new()
    {
      IsHerbal = invariant.GetBooleanValue(Medicines.IsHerbal)
    };

    double? healing = invariant.TryGetNumberValue(Medicines.Healing);
    if (healing.HasValue)
    {
      medicine.Healing = new HealingModel(
        (int)healing.Value,
        invariant.GetBooleanValue(Medicines.IsHealingPercentage),
        invariant.GetBooleanValue(Medicines.Revive));
    }

    IReadOnlyCollection<string> statusConditions = invariant.GetSelectValue(Medicines.StatusCondition);
    bool allConditions = invariant.GetBooleanValue(Medicines.AllConditions);
    if (statusConditions.Count > 0 || allConditions)
    {
      medicine.Status = new StatusHealingModel(
        condition: statusConditions.Count < 1 ? null : PokemonConverter.Instance.ToStatusCondition(statusConditions.Single()),
        allConditions);
    }

    double? powerPoints = invariant.TryGetNumberValue(Medicines.PowerPoints);
    if (powerPoints.HasValue)
    {
      medicine.PowerPoints = new PowerPointRestoreModel(
        (int)powerPoints.Value,
        invariant.GetBooleanValue(Medicines.IsPowerPointPercentage),
        invariant.GetBooleanValue(Medicines.AllMoves));
    }

    Medicine = JsonSerializer.Serialize(medicine); // TODO(fpion): use custom serializer
  }
  public void Update(PokeBallPublished published)
  {
    Update(published.Event, published.Invariant, published.Locale, PokeBalls.Price, ItemCategory.PokeBall, PokeBalls.Sprite, PokeBalls.Url, PokeBalls.Notes);

    ContentLocale invariant = published.Invariant;
    PokeBallModel pokeBall = new(
      invariant.GetNumberValue(PokeBalls.CatchMultiplier),
      invariant.GetBooleanValue(PokeBalls.Heal),
      (int)invariant.GetNumberValue(PokeBalls.BaseFriendship),
      (int)invariant.GetNumberValue(PokeBalls.FriendshipMultiplier));
    PokeBall = JsonSerializer.Serialize(pokeBall); // TODO(fpion): use custom serializer
  }
  public void Update(TechnicalMachinePublished published)
  {
    Update(published.Event, published.Invariant, published.Locale, TechnicalMachines.Price,
      ItemCategory.TechnicalMachine, TechnicalMachines.Sprite, TechnicalMachines.Url, TechnicalMachines.Notes);
  }
  private void Update(DomainEvent @event, ContentLocale invariant, ContentLocale locale, Guid price, ItemCategory category, Guid sprite, Guid url, Guid notes)
  {
    Update(@event);

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    Price = (int)invariant.GetNumberValue(price);

    Category = category;

    Sprite = invariant.TryGetStringValue(sprite);

    Url = locale.TryGetStringValue(url);
    Notes = locale.TryGetStringValue(notes);
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
