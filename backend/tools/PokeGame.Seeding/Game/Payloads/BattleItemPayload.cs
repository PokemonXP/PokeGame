using CsvHelper.Configuration;

namespace PokeGame.Seeding.Game.Payloads;

internal class BattleItemPayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int Price { get; set; }

  public StatisticChangesPayload StatisticChanges { get; set; } = new();
  public int CriticalChange { get; set; }
  public int GuardTurns { get; set; }

  public string? Sprite { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is BattleItemPayload battleitem && battleitem.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<BattleItemPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);

      References<StatisticChangesMap>(x => x.StatisticChanges);
      Map(x => x.CriticalChange).Index(12).Default(0);
      Map(x => x.GuardTurns).Index(13).Default(0);

      Map(x => x.Sprite).Index(14);

      Map(x => x.Url).Index(15);
      Map(x => x.Notes).Index(16);
    }
  }

  private class StatisticChangesMap : ClassMap<StatisticChangesPayload>
  {
    public StatisticChangesMap()
    {
      Map(x => x.Attack).Index(5).Default(0);
      Map(x => x.Defense).Index(6).Default(0);
      Map(x => x.SpecialAttack).Index(7).Default(0);
      Map(x => x.SpecialDefense).Index(8).Default(0);
      Map(x => x.Speed).Index(9).Default(0);
      Map(x => x.Accuracy).Index(10).Default(0);
      Map(x => x.Evasion).Index(11).Default(0);
    }
  }
}
