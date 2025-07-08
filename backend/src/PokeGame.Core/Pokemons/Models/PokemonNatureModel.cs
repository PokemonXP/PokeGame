namespace PokeGame.Core.Pokemons.Models;

public record PokemonNatureModel : IPokemonNature
{
  public string Name { get; set; }
  public PokemonStatistic? IncreasedStatistic { get; set; }
  public PokemonStatistic? DecreasedStatistic { get; set; }
  public Flavor? FavoriteFlavor { get; set; }
  public Flavor? DislikedFlavor { get; set; }

  public PokemonNatureModel() : this(string.Empty)
  {
  }

  public PokemonNatureModel(
    string name,
    PokemonStatistic? increasedStatistics = null,
    PokemonStatistic? decreasedStatistics = null,
    Flavor? favoriteFlavor = null,
    Flavor? dislikedFlavor = null)
  {
    Name = name;
    IncreasedStatistic = increasedStatistics;
    DecreasedStatistic = decreasedStatistics;
    FavoriteFlavor = favoriteFlavor;
    DislikedFlavor = dislikedFlavor;
  }

  public PokemonNatureModel(IPokemonNature nature) : this(nature.Name, nature.IncreasedStatistic, nature.DecreasedStatistic, nature.FavoriteFlavor, nature.DislikedFlavor)
  {
  }
}
