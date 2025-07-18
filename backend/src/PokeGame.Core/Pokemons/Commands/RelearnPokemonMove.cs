using FluentValidation;
using Krakenar.Core;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Core.Pokemons.Commands;

internal record RelearnPokemonMove(Guid Id, RelearnPokemonMovePayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="MoveNotFoundException"></exception>
/// <exception cref="PokemonNeverLearnedMoveException"></exception>
/// <exception cref="ValidationException"></exception>
internal class RelearnPokemonMoveHandler : ICommandHandler<RelearnPokemonMove, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMoveManager _moveManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public RelearnPokemonMoveHandler(
    IApplicationContext applicationContext,
    IMoveManager moveManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _moveManager = moveManager;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(RelearnPokemonMove command, CancellationToken cancellationToken)
  {
    //RelearnPokemonMovePayload payload = command.Payload;
    //new RelearnPokemonMoveValidator().ValidateAndThrow(payload);

    //PokemonId pokemonId = new(command.Id);
    //Pokemon? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    //if (pokemon is null)
    //{
    //  return null;
    //}

    //MoveModel move = await _moveManager.FindAsync(payload.Move, nameof(payload.Move), cancellationToken);
    //MoveId moveId = move.GetMoveId(_applicationContext.RealmId);
    //if (!pokemon.RelearnMove(moveId, payload.Position, _applicationContext.ActorId))
    //{
    //  throw new PokemonNeverLearnedMoveException(pokemon, moveId, nameof(payload.Move));
    //}
    //await _pokemonRepository.SaveAsync(pokemon, cancellationToken);

    //return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);

    await Task.Delay(1000, cancellationToken);
    return null;
  }
}
