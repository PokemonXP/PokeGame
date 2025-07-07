using Krakenar.Contracts.Search;

namespace PokeGame.Core.Regions.Models;

public record SearchRegionsPayload : SearchPayload
{
  public new List<RegionSortOption> Sort { get; set; } = [];
}
