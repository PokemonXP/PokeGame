using Krakenar.Core;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Commands;

internal record DeletePokemon(Guid Id) : ICommand<PokemonModel?>;

internal class DeletePokemonHandler : ICommandHandler<DeletePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public DeletePokemonHandler(IApplicationContext applicationContext, IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(DeletePokemon command, CancellationToken cancellationToken)
  {
    PokemonId pokemonId = new(command.Id);
    Pokemon2? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }
    PokemonModel model = await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);

    pokemon.Delete(_applicationContext.ActorId);
    await _pokemonRepository.SaveAsync(pokemon, cancellationToken);

    return model;
  }
}
