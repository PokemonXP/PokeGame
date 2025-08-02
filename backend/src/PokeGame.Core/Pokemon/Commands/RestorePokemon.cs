using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Commands;

internal record RestorePokemon(Guid Id) : ICommand<PokemonModel?>;

internal class RestorePokemonHandler : ICommandHandler<RestorePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public RestorePokemonHandler(IApplicationContext applicationContext, IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(RestorePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    pokemon.Restore(actorId);

    await _pokemonRepository.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
