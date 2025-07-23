using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Commands;

internal record WithdrawPokemon(Guid Id) : ICommand<PokemonModel?>;

/// <exception cref="ValidationException"></exception>
internal class WithdrawPokemonHandler : ICommandHandler<WithdrawPokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public WithdrawPokemonHandler(IApplicationContext applicationContext, IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(WithdrawPokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    if (pokemon.Ownership is null || pokemon.Slot is null)
    {
      ValidationFailure failure = new(nameof(command.Id), "The Pokémon is not owned by any trainer.", pokemon.Id.ToGuid())
      {
        ErrorCode = "PokemonHasNoOwner"
      };
      throw new ValidationException([failure]);
    }
    else if (pokemon.Slot.Box is null)
    {
      ValidationFailure failure = new(nameof(command.Id), "The Pokémon is already in the party.", pokemon.Id.ToGuid())
      {
        ErrorCode = "PokemonAlreadyInParty"
      };
      throw new ValidationException([failure]);
    }

    TrainerId trainerId = pokemon.Ownership.TrainerId;
    Storage storage = await _pokemonQuerier.GetStorageAsync(trainerId, cancellationToken);

    Position? position = storage.GetFirstPartyEmptySlot();
    if (position is null)
    {
      ValidationFailure failure = new("TrainerId", "The trainer party is already full.", trainerId.ToGuid())
      {
        ErrorCode = "TrainerPartyFull"
      };
      throw new ValidationException([failure]);
    }
    pokemon.Withdraw(position, actorId);

    await _pokemonRepository.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
