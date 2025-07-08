using CsvHelper.Configuration;

namespace PokeGame.Core.Pokemons;

internal record NatureInfo
{
  public int Number { get; set; }
  public string Name { get; set; } = string.Empty;

  public PokemonStatistic IncreasedStatistic { get; set; }
  public PokemonStatistic DecreasedStatistic { get; set; }

  public Flavor FavoriteFlavor { get; set; }
  public Flavor DislikedFlavor { get; set; }

  public class Map : ClassMap<NatureInfo>
  {
    public Map()
    {
      Map(x => x.Number).Index(0);
      Map(x => x.Name).Index(1);

      Map(x => x.IncreasedStatistic).Index(2);
      Map(x => x.DecreasedStatistic).Index(3);

      Map(x => x.FavoriteFlavor).Index(4);
      Map(x => x.DislikedFlavor).Index(5);
    }
  }
}
