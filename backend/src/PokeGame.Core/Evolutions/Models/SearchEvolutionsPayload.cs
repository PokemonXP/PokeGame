using Krakenar.Contracts.Search;

namespace PokeGame.Core.Evolutions.Models;

public record SearchEvolutionsPayload : SearchPayload
{
  public Guid? SourceId { get; set; }
  public Guid? TargetId { get; set; }
  public EvolutionTrigger? Trigger { get; set; }

  public new List<EvolutionSortOption> Sort { get; set; } = [];
}
