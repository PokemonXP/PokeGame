using PokeGame.Core.Regions.Models;

namespace PokeGame.Core.Species.Models;

public record RegionalNumberModel
{
  public RegionModel Region { get; set; } = new();
  public int Number { get; set; }

  public RegionalNumberModel()
  {
  }

  public RegionalNumberModel(RegionModel region, int number)
  {
    Region = region;
    Number = number;
  }
}
