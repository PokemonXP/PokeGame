using Krakenar.Contracts.Search;

namespace PokeGame.Core.Varieties.Models;

public record SearchVarietiesPayload : SearchPayload
{
  public new List<VarietySortOption> Sort { get; set; } = [];
}
