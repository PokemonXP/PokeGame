using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.Infrastructure;
using PokeGame.Infrastructure.Data;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class MoveEntity : AggregateEntity
{
  public int MoveId { get; private set; }
  public Guid Id { get; private set; }

  public PokemonType Type { get; private set; }
  public MoveCategory Category { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public int Accuracy { get; private set; }
  public int Power { get; private set; }
  public int PowerPoints { get; private set; }

  public StatusCondition? StatusCondition { get; private set; }
  public int StatusChance { get; private set; }
  public string? VolatileConditions { get; private set; }

  public int AttackChange { get; private set; }
  public int DefenseChange { get; private set; }
  public int SpecialAttackChange { get; private set; }
  public int SpecialDefenseChange { get; private set; }
  public int SpeedChange { get; private set; }
  public int AccuracyChange { get; private set; }
  public int EvasionChange { get; private set; }
  public int CriticalChange { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public MoveEntity(MovePublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Update(published);
  }

  private MoveEntity() : base()
  {
  }

  public void Update(MovePublished published)
  {
    ContentLocale invariant = published.Invariant;
    ContentLocale locale = published.Locale;

    Update(published.Event);

    Type = Enum.Parse<PokemonType>(invariant.FindSelectValue(Moves.Type).Single().Capitalize());
    Category = Enum.Parse<MoveCategory>(invariant.FindSelectValue(Moves.Category).Single().Capitalize());

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    Accuracy = (int)invariant.GetNumberValue(Moves.Accuracy);
    Power = (int)invariant.GetNumberValue(Moves.Power);
    PowerPoints = (int)invariant.FindNumberValue(Moves.PowerPoints);

    IReadOnlyCollection<string>? statusConditions = invariant.TryGetSelectValue(Moves.InflictedCondition);
    StatusCondition = statusConditions is null || statusConditions.Count < 1 ? null : Enum.Parse<StatusCondition>(statusConditions.Single().Capitalize());
    StatusChance = (int)invariant.GetNumberValue(Moves.StatusChance);

    IReadOnlyCollection<string>? volatileConditions = invariant.TryGetSelectValue(Moves.VolatileConditions);
    VolatileConditions = volatileConditions is null || volatileConditions.Count < 1
      ? null
      : JsonSerializer.Serialize(volatileConditions.Select(value => Enum.Parse<VolatileCondition>(value.Capitalize()).ToString()).Distinct());

    AttackChange = (int)invariant.GetNumberValue(Moves.AttackChange);
    DefenseChange = (int)invariant.GetNumberValue(Moves.DefenseChange);
    SpecialAttackChange = (int)invariant.GetNumberValue(Moves.SpecialAttackChange);
    SpecialDefenseChange = (int)invariant.GetNumberValue(Moves.SpecialDefenseChange);
    SpeedChange = (int)invariant.GetNumberValue(Moves.SpeedChange);
    AccuracyChange = (int)invariant.GetNumberValue(Moves.AccuracyChange);
    EvasionChange = (int)invariant.GetNumberValue(Moves.EvasionChange);
    CriticalChange = (int)invariant.GetNumberValue(Moves.CriticalChange);

    Url = locale.TryGetStringValue(Moves.Url);
    Notes = locale.TryGetStringValue(Moves.Notes);
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
