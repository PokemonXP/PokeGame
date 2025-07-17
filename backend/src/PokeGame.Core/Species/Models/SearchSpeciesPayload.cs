using Krakenar.Contracts.Search;

namespace PokeGame.Core.Species.Models;

public record SearchSpeciesPayload : SearchPayload
{
  public PokemonCategory? Category { get; set; }
  public EggGroup? EggGroup { get; set; }
  public GrowthRate? GrowthRate { get; set; }
  public Guid? RegionId { get; set; }

  public new List<SpeciesSortOption> Sort { get; set; } = [];
}
