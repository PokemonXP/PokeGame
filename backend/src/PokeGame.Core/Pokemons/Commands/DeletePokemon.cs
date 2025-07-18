using Krakenar.Core;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Core.Pokemons.Commands;

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
    //PokemonId pokemonId = new(command.Id);
    //Pokemon? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    //if (pokemon is null)
    //{
    //  return null;
    //}
    //PokemonModel model = await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);

    //pokemon.Delete(_applicationContext.ActorId);
    //await _pokemonRepository.SaveAsync(pokemon, cancellationToken);

    //return model;

    await Task.Delay(1000, cancellationToken);
    return null;
  }
}
