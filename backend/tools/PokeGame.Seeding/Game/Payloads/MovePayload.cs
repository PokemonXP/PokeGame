using CsvHelper.Configuration;
using PokeGame.Core;
using PokeGame.Core.Moves;

namespace PokeGame.Seeding.Game.Payloads;

internal class MovePayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public PokemonType Type { get; set; }
  public MoveCategory Category { get; set; }

  public int Accuracy { get; set; }
  public int Power { get; set; }
  public int PowerPoints { get; set; }

  public InflictedStatusPayload? InflictedStatus { get; set; }
  public string? VolatileConditions { get; set; }

  public StatisticChangesPayload StatisticChanges { get; set; } = new();
  public int CriticalChange { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is MovePayload move && move.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<MovePayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(3).Default(string.Empty);
      Map(x => x.DisplayName).Index(4);
      Map(x => x.Description).Index(5);

      Map(x => x.Type).Index(1).Default(default(PokemonType));
      Map(x => x.Category).Index(2).Default(default(MoveCategory));

      Map(x => x.Accuracy).Index(6).Default(0);
      Map(x => x.Power).Index(7).Default(0);
      Map(x => x.PowerPoints).Index(8).Default(0);

      Map(x => x.InflictedStatus).Convert(args =>
      {
        StatusCondition? condition = args.Row.GetField<StatusCondition?>(9);
        int? chance = args.Row.GetField<int?>(10);
        return condition.HasValue ? new InflictedStatusPayload(condition.Value, chance ?? 0) : null;
      });
      Map(x => x.VolatileConditions).Index(11);

      References<StatisticChangesMap>(x => x.StatisticChanges);
      Map(x => x.CriticalChange).Index(19).Default(0);

      Map(x => x.Url).Index(20);
      Map(x => x.Notes).Index(21);
    }
  }

  private class StatisticChangesMap : ClassMap<StatisticChangesPayload>
  {
    public StatisticChangesMap()
    {
      Map(x => x.Attack).Index(12).Default(0);
      Map(x => x.Defense).Index(13).Default(0);
      Map(x => x.SpecialAttack).Index(14).Default(0);
      Map(x => x.SpecialDefense).Index(15).Default(0);
      Map(x => x.Speed).Index(16).Default(0);
      Map(x => x.Accuracy).Index(17).Default(0);
      Map(x => x.Evasion).Index(18).Default(0);
    }
  }
}
