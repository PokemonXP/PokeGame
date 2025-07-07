using Krakenar.Core;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Core.Pokemons;

public interface IPokemonQuerier
{
  Task<PokemonId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<PokemonModel> ReadAsync(Pokemon pokemon, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(PokemonId id, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
}
