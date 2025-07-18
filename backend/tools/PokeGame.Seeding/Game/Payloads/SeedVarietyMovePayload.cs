using CsvHelper.Configuration;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedVarietyMovePayload
{
  public string Variety { get; set; } = string.Empty;
  public string Move { get; set; } = string.Empty;
  public int Level { get; set; }

  public class Map : ClassMap<SeedVarietyMovePayload>
  {
    public Map()
    {
      Map(x => x.Variety).Index(0).Default(string.Empty);
      Map(x => x.Move).Index(2).Default(string.Empty);
      Map(x => x.Level).Index(1).Default(0);
    }
  }
}
