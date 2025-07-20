using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemon.Validators;

namespace PokeGame.Core.Pokemon.Commands;

internal record RememberPokemonMove(Guid PokemonId, Guid MoveId, RememberPokemonMovePayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="ValidationException"></exception>
internal class RememberPokemonMoveHandler : ICommandHandler<RememberPokemonMove, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public RememberPokemonMoveHandler(
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

  public async Task<PokemonModel?> HandleAsync(RememberPokemonMove command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    RememberPokemonMovePayload payload = command.Payload;
    new RememberPokemonMoveValidator().ValidateAndThrow(payload);

    PokemonId pokemonId = new(command.PokemonId);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    MoveId moveId = new(command.MoveId);
    if (!pokemon.RememberMove(moveId, payload.Position, actorId))
    {
      return null;
    }
    await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
