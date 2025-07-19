using FluentValidation;
using Krakenar.Core;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Core.Pokemons.Commands;

internal record SwitchPokemonMoves(Guid Id, SwitchPokemonMovesPayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="ValidationException"></exception>
internal class SwitchPokemonMovesHandler : ICommandHandler<SwitchPokemonMoves, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public SwitchPokemonMovesHandler(IApplicationContext applicationContext, IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(SwitchPokemonMoves command, CancellationToken cancellationToken)
  {
    //SwitchPokemonMovesPayload payload = command.Payload;
    //new SwitchPokemonMovesValidator().ValidateAndThrow(payload);

    //PokemonId pokemonId = new(command.Id);
    //Pokemon? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    //if (pokemon is null)
    //{
    //  return null;
    //}

    //pokemon.SwitchMoves(payload.Source, payload.Destination, _applicationContext.ActorId);
    //await _pokemonRepository.SaveAsync(pokemon, cancellationToken);

    //return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);

    await Task.Delay(1000, cancellationToken);
    return null;
  }
}
