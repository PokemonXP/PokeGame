using Krakenar.Contracts.Search;

namespace PokeGame.Core.Abilities.Models;

public record SearchAbilitiesPayload : SearchPayload
{
  public new List<AbilitySortOption> Sort { get; set; } = [];
}
