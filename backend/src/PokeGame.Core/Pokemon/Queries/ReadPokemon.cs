using Krakenar.Contracts;
using Krakenar.Core;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Queries;

internal record ReadPokemon(Guid? Id, string? UniqueName) : IQuery<PokemonModel?>;

internal class ReadPokemonHandler : IQueryHandler<ReadPokemon, PokemonModel?>
{
  private readonly IPokemonQuerier _pokemonQuerier;

  public ReadPokemonHandler(IPokemonQuerier pokemonQuerier)
  {
    _pokemonQuerier = pokemonQuerier;
  }

  public async Task<PokemonModel?> HandleAsync(ReadPokemon query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, PokemonModel> foundPokemon = new(capacity: 2);

    if (query.Id.HasValue)
    {
      PokemonModel? pokemon = await _pokemonQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (pokemon is not null)
      {
        foundPokemon[pokemon.Id] = pokemon;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      PokemonModel? pokemon = await _pokemonQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (pokemon is not null)
      {
        foundPokemon[pokemon.Id] = pokemon;
      }
    }

    if (foundPokemon.Count > 1)
    {
      throw TooManyResultsException<PokemonModel>.ExpectedSingle(foundPokemon.Count);
    }

    return foundPokemon.SingleOrDefault().Value;
  }
}
