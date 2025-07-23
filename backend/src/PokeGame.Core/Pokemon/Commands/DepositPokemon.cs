using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Commands;

internal record DepositPokemon(Guid Id) : ICommand<PokemonModel?>;

/// <exception cref="ValidationException"></exception>
internal class DepositPokemonHandler : ICommandHandler<DepositPokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public DepositPokemonHandler(IApplicationContext applicationContext, IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(DepositPokemon command, CancellationToken cancellationToken)
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
    else if (pokemon.Slot.Box is not null)
    {
      ValidationFailure failure = new(nameof(command.Id), "The Pokémon is already in a box.", pokemon.Id.ToGuid())
      {
        ErrorCode = "PokemonIsAlreadyInABox"
      };
      throw new ValidationException([failure]);
    }

    TrainerId trainerId = pokemon.Ownership.TrainerId;
    Storage storage = await _pokemonQuerier.GetStorageAsync(trainerId, cancellationToken);
    IEnumerable<PokemonId> pokemonIds = storage.Party.Except([pokemon.Id]);
    IReadOnlyCollection<Specimen> partyPokemon = await _pokemonRepository.LoadAsync(pokemonIds, cancellationToken);
    if (!pokemon.IsEgg && !partyPokemon.Any(p => !p.IsEgg))
    {
      ValidationFailure failure = new("TrainerId", "The trainer party must contain at least one other hatched Pokémon.", trainerId.ToGuid())
      {
        ErrorCode = "CannotEmptyTrainerParty"
      };
      throw new ValidationException([failure]);
    }
    foreach (Specimen member in partyPokemon)
    {
      if (member.Slot is not null && member.Slot.Box is null && member.Slot.Position.Value > pokemon.Slot.Position.Value)
      {
        PokemonSlot newSlot = new(new Position(member.Slot.Position.Value - 1), Box: null);
        member.Move(newSlot, actorId);
      }
    }

    PokemonSlot slot = storage.GetFirstBoxEmptySlot();
    pokemon.Deposit(slot, actorId);

    await _pokemonRepository.SaveAsync(new[] { pokemon }.Concat(partyPokemon), cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
