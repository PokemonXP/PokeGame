using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Commands;

internal record ReleasePokemon(Guid Id) : ICommand<PokemonModel?>;

/// <exception cref="CannotReleasePartyPokemonException"></exception>
internal class ReleasePokemonHandler : ICommandHandler<ReleasePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public ReleasePokemonHandler(
    IApplicationContext applicationContext,
    IPokemonManager pokemonManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonManager = pokemonManager;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(ReleasePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    pokemon.Release(actorId);

    await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
