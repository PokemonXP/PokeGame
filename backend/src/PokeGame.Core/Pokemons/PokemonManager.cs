using PokeGame.Core.Pokemons.Events;

namespace PokeGame.Core.Pokemons;

internal interface IPokemonManager
{
  Task SaveAsync(Pokemon2 pokemon, CancellationToken cancellationToken = default);
}

internal class PokemonManager : IPokemonManager
{
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public PokemonManager(IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task SaveAsync(Pokemon2 pokemon, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = pokemon.Changes.Any(change => change is PokemonCreated || change is PokemonUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      PokemonId? conflictId = await _pokemonQuerier.FindIdAsync(pokemon.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(pokemon.Id))
      {
        throw new UniqueNameAlreadyUsedException(pokemon, conflictId.Value);
      }
    }

    await _pokemonRepository.SaveAsync(pokemon, cancellationToken);
  }
}
