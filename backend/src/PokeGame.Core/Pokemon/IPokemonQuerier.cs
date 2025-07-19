using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon;

public interface IPokemonQuerier
{
  Task<PokemonId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<PokemonModel> ReadAsync(Specimen pokemon, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(PokemonId id, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<PokemonModel>> SearchAsync(SearchPokemonPayload payload, CancellationToken cancellationToken = default);
}
