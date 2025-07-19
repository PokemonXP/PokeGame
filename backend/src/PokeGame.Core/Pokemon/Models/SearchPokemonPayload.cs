using Krakenar.Contracts.Search;

namespace PokeGame.Core.Pokemon.Models;

public record SearchPokemonPayload : SearchPayload
{
  public new List<PokemonSortOption> Sort { get; set; } = [];
}
