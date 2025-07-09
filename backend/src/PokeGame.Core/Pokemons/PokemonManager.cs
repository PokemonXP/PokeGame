using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Pokemons.Events;

namespace PokeGame.Core.Pokemons;

internal interface IPokemonManager
{
  Task<FormModel> FindFormAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken = default);

  Task SaveAsync(Pokemon pokemon, CancellationToken cancellationToken = default);
}

internal class PokemonManager : IPokemonManager
{
  private readonly IFormQuerier _formQuerier;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public PokemonManager(IFormQuerier formQuerier, IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _formQuerier = formQuerier;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<FormModel> FindFormAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken)
  {
    FormModel? form = null;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      form = await _formQuerier.ReadAsync(id, cancellationToken);
    }
    return form ?? await _formQuerier.ReadAsync(idOrUniqueName, cancellationToken) ?? throw new FormNotFoundException(idOrUniqueName, propertyName);
  }

  public async Task SaveAsync(Pokemon pokemon, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = pokemon.Changes.Any(change => change is PokemonCreated || change is PokemonUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      PokemonId? conflictId = await _pokemonQuerier.FindIdAsync(pokemon.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(pokemon.Id))
      {
        throw new PokemonUniqueNameAlreadyUsedException(pokemon, conflictId.Value);
      }
    }

    await _pokemonRepository.SaveAsync(pokemon, cancellationToken);
  }
}
