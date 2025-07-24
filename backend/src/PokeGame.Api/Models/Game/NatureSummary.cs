using PokeGame.Core;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Api.Models.Game;

public record NatureSummary
{
  public string Name { get; set; }
  public PokemonStatistic? IncreasedStatistic { get; set; }
  public PokemonStatistic? DecreasedStatistic { get; set; }
  public Flavor? FavoriteFlavor { get; set; }
  public Flavor? DislikedFlavor { get; set; }

  public NatureSummary() : this(string.Empty)
  {
  }

  public NatureSummary(string name)
  {
    Name = name;
  }

  public NatureSummary(PokemonNatureModel nature) : this(nature.Name)
  {
    IncreasedStatistic = nature.IncreasedStatistic;
    DecreasedStatistic = nature.DecreasedStatistic;
    FavoriteFlavor = nature.FavoriteFlavor;
    DislikedFlavor = nature.DislikedFlavor;
  }
}
