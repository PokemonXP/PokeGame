using CsvHelper.Configuration;

namespace PokeGame.Core.Pokemon;

internal record LevelExperience
{
  public int Level { get; set; }

  public int Erratic { get; set; }
  public int Fast { get; set; }
  public int MediumFast { get; set; }
  public int MediumSlow { get; set; }
  public int Slow { get; set; }
  public int Fluctuating { get; set; }

  public class Map : ClassMap<LevelExperience>
  {
    public Map()
    {
      Map(x => x.Level).Index(0);

      Map(x => x.Erratic).Index(1);
      Map(x => x.Fast).Index(2);
      Map(x => x.MediumFast).Index(3);
      Map(x => x.MediumSlow).Index(4);
      Map(x => x.Slow).Index(5);
      Map(x => x.Fluctuating).Index(6);
    }
  }
}
