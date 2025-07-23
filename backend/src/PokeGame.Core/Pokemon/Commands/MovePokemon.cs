using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemon.Validators;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Commands;

internal record MovePokemon(Guid Id, MovePokemonPayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="ValidationException"></exception>
internal class MovePokemonHandler : ICommandHandler<MovePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public MovePokemonHandler(IApplicationContext applicationContext, IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(MovePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    MovePokemonPayload payload = command.Payload;
    new MovePokemonValidator().ValidateAndThrow(payload);

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

    TrainerId trainerId = pokemon.Ownership.TrainerId;
    Storage storage = await _pokemonQuerier.GetStorageAsync(trainerId, cancellationToken);
    PokemonSlot slot = new(new Position(payload.Position), new Box(payload.Box));
    if (!storage.IsEmpty(slot))
    {
      ValidationFailure failure = new(nameof(Specimen.Slot), "The slot is not empty.", $"{{Position:{slot.Position},Box:{slot.Box}}}")
      {
        ErrorCode = "PokemonSlotNotEmpty"
      };
      throw new ValidationException([failure]);
    }

    if (pokemon.Slot.Box is null)
    {
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

      pokemon.Move(slot, actorId);

      foreach (Specimen member in partyPokemon)
      {
        if (member.Slot is not null && member.Slot.Box is null && member.Slot.Position.Value > pokemon.Slot.Position.Value)
        {
          PokemonSlot newSlot = new(new Position(member.Slot.Position.Value - 1), Box: null);
          member.Move(newSlot, actorId);
        }
      }

      await _pokemonRepository.SaveAsync(new[] { pokemon }.Concat(partyPokemon), cancellationToken);
    }
    else
    {
      pokemon.Move(slot, actorId);

      await _pokemonRepository.SaveAsync(pokemon, cancellationToken);
    }

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
