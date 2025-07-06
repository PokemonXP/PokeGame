using CsvHelper.Configuration;
using PokeGame.Core;

namespace PokeGame.Seeding.Game.Payloads;

internal class BerryPayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int Price { get; set; }

  public int Healing { get; set; }
  public bool IsHealingPercentage { get; set; }

  public StatusCondition? StatusCondition { get; set; }
  public bool CureConfusion { get; set; }
  public bool CureNonVolatileConditions { get; set; }

  public int PowerPoints { get; set; }

  public StatisticChangesPayload StatisticChanges { get; set; } = new();
  public int CriticalChange { get; set; }

  public PokemonStatistic? LowerEffortValues { get; set; }

  public bool RaiseFriendship { get; set; }

  public string? Sprite { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is BerryPayload Berry && Berry.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<BerryPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0);

      Map(x => x.UniqueName).Index(1);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);

      Map(x => x.Healing).Index(5).Default(0);
      Map(x => x.IsHealingPercentage).Index(6).Default(false);

      Map(x => x.StatusCondition).Index(7);
      Map(x => x.CureConfusion).Index(8).Default(false);
      Map(x => x.CureNonVolatileConditions).Index(9).Default(false);

      Map(x => x.PowerPoints).Index(10).Default(0);

      References<StatisticChangesMap>(x => x.StatisticChanges);
      Map(x => x.CriticalChange).Index(18).Default(0);

      Map(x => x.LowerEffortValues).Index(19);
      Map(x => x.RaiseFriendship).Index(20).Default(false);

      Map(x => x.Sprite).Index(21);

      Map(x => x.Url).Index(22);
      Map(x => x.Notes).Index(23);
    }
  }

  private class StatisticChangesMap : ClassMap<StatisticChangesPayload>
  {
    public StatisticChangesMap()
    {
      Map(x => x.Attack).Index(11).Default(0);
      Map(x => x.Defense).Index(12).Default(0);
      Map(x => x.SpecialAttack).Index(13).Default(0);
      Map(x => x.SpecialDefense).Index(14).Default(0);
      Map(x => x.Speed).Index(15).Default(0);
      Map(x => x.Accuracy).Index(16).Default(0);
      Map(x => x.Evasion).Index(17).Default(0);
    }
  }
}
