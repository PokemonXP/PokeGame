using Krakenar.Contracts.Search;

namespace PokeGame.Core.Pokemons.Models;

public record SearchPokemonPayload : SearchPayload
{
  public Guid? TrainerId { get; set; }

  public new List<PokemonSortOption> Sort { get; set; } = [];
}
