using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemon.Validators;

namespace PokeGame.Core.Pokemon.Commands;

internal record SwitchPokemonMoves(Guid Id, SwitchPokemonMovesPayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="ValidationException"></exception>
internal class SwitchPokemonMovesHandler : ICommandHandler<SwitchPokemonMoves, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public SwitchPokemonMovesHandler(
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

  public async Task<PokemonModel?> HandleAsync(SwitchPokemonMoves command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    SwitchPokemonMovesPayload payload = command.Payload;
    new SwitchPokemonMovesValidator().ValidateAndThrow(payload);

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    pokemon.SwitchMoves(payload.Source, payload.Destination, actorId);
    await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
