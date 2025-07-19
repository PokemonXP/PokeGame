using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Queries;

internal record SearchPokemon(SearchPokemonPayload Payload) : IQuery<SearchResults<PokemonModel>>;

internal class SearchPokemonHandler : IQueryHandler<SearchPokemon, SearchResults<PokemonModel>>
{
  private readonly IPokemonQuerier _pokemonQuerier;

  public SearchPokemonHandler(IPokemonQuerier pokemonQuerier)
  {
    _pokemonQuerier = pokemonQuerier;
  }

  public async Task<SearchResults<PokemonModel>> HandleAsync(SearchPokemon query, CancellationToken cancellationToken)
  {
    return await _pokemonQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
