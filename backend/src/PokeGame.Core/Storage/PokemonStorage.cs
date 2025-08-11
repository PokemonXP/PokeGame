using FluentValidation;
using FluentValidation.Results;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Storage.Events;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Storage;

public class PokemonStorage : AggregateRoot
{
  public new PokemonStorageId Id => new(base.Id);
  public TrainerId TrainerId => Id.TrainerId;

  private readonly Dictionary<PokemonSlot, PokemonId> _pokemon = [];
  private readonly Dictionary<PokemonId, PokemonSlot> _slots = [];

  public PokemonStorage() : base()
  {
  }

  public PokemonStorage(Trainer trainer) : this(trainer.Id)
  {
  }
  public PokemonStorage(TrainerId trainerId) : base(new PokemonStorageId(trainerId).StreamId)
  {
  }

  public IReadOnlyCollection<PokemonId> GetParty() => _slots
    .Where(x => x.Value.Box is null)
    .OrderBy(x => x.Value.Position.Value)
    .Select(x => x.Key).ToList().AsReadOnly();

  protected virtual void Handle(PokemonStored @event)
  {
    if (_slots.TryGetValue(@event.PokemonId, out PokemonSlot? previousSlot))
    {
      _pokemon.Remove(previousSlot);
    }
    _pokemon[@event.Slot] = @event.PokemonId;
    _slots[@event.PokemonId] = @event.Slot;
  }

  public void Deposit(Specimen pokemon, IReadOnlyDictionary<PokemonId, Specimen> party, ActorId? actorId = null)
  {
    if (!_slots.TryGetValue(pokemon.Id, out PokemonSlot? previousSlot))
    {
      throw new ArgumentException($"The Pokémon '{pokemon}' was not found in trainer's 'Id={TrainerId}' storage.", nameof(pokemon));
    }
    else if (previousSlot.Box is not null)
    {
      throw new ArgumentException($"The Pokémon '{pokemon}' is not in trainer's 'Id={TrainerId}' party.", nameof(pokemon));
    }

    IReadOnlyCollection<PokemonId> partyIds = GetParty();
    EnsurePartyIsNotEmpty(party, [pokemon.Id], partyIds);

    PokemonSlot newSlot = FindFirstBoxAvailable();
    pokemon.Deposit(newSlot, actorId);
    Raise(new PokemonStored(pokemon.Id, newSlot), actorId);

    ShiftPartyMembers(previousSlot, party, [pokemon.Id], partyIds, actorId);
  }

  public void Move(Specimen pokemon, PokemonSlot slot, IReadOnlyDictionary<PokemonId, Specimen> party, ActorId? actorId = null)
  {
    if (!_slots.TryGetValue(pokemon.Id, out PokemonSlot? previousSlot))
    {
      throw new ArgumentException($"The Pokémon '{pokemon}' was not found in trainer's 'Id={TrainerId}' storage.", nameof(pokemon));
    }
    else if (slot.Box is null)
    {
      throw new ArgumentException("The slot must not be in the party.", nameof(slot));
    }
    else if (_pokemon.TryGetValue(slot, out PokemonId pokemonId))
    {
      if (pokemonId == pokemon.Id)
      {
        return;
      }

      ValidationFailure failure = new(nameof(TrainerId), "The specified Pokémon slot is not empty.", TrainerId.ToGuid())
      {
        CustomState = new
        {
          Position = slot.Position.Value,
          Box = slot.Box.Value
        },
        ErrorCode = "PokemonSlotNotEmpty"
      };
      throw new ValidationException([failure]);
    }

    if (pokemon.IsHatchedInParty)
    {
      EnsurePartyIsNotEmpty(party, [pokemon.Id]);
    }

    pokemon.Move(slot, actorId);
    Raise(new PokemonStored(pokemon.Id, slot), actorId);

    if (previousSlot.Box is null)
    {
      ShiftPartyMembers(previousSlot, party, [pokemon.Id], partyIds: null, actorId);
    }
  }

  public void Release(Specimen pokemon, IReadOnlyDictionary<PokemonId, Specimen> party, ActorId? actorId = null)
  {
    if (!_slots.ContainsKey(pokemon.Id))
    {
      throw new ArgumentException($"The Pokémon '{pokemon}' was not found in trainer's 'Id={TrainerId}' storage.", nameof(pokemon));
    }
    else if (pokemon.IsHatchedInParty)
    {
      EnsurePartyIsNotEmpty(party, [pokemon.Id]);
    }

    pokemon.Release(actorId);
    Remove(pokemon, actorId);
  }

  public bool Remove(Specimen pokemon, ActorId? actorId = null)
  {
    if (!_slots.ContainsKey(pokemon.Id))
    {
      return false;
    }

    Raise(new PokemonRemoved(pokemon.Id), actorId);
    return true;
  }
  protected virtual void Handle(PokemonRemoved @event)
  {
    if (_slots.TryGetValue(@event.PokemonId, out PokemonSlot? slot))
    {
      _pokemon.Remove(slot);
    }
    _slots.Remove(@event.PokemonId);
  }

  public void Store(Specimen pokemon, ActorId? actorId = null)
  {
    if (pokemon.Ownership is null || pokemon.Ownership.TrainerId != TrainerId)
    {
      throw new ArgumentException($"The Pokémon owner trainer 'Id={pokemon.Ownership?.TrainerId.Value ?? "<null>"}' must be '{TrainerId}'.", nameof(pokemon));
    }
    else if (!_slots.ContainsKey(pokemon.Id))
    {
      PokemonSlot slot = FindFirstAvailable();
      pokemon.Move(slot, actorId);
      Raise(new PokemonStored(pokemon.Id, slot), actorId);
    }
  }

  public void Swap(Specimen source, Specimen destination, IReadOnlyDictionary<PokemonId, Specimen> party, ActorId? actorId = null)
  {
    if (!_slots.ContainsKey(source.Id))
    {
      throw new ArgumentException($"The Pokémon '{source}' was not found in trainer's 'Id={TrainerId}' storage.", nameof(source));
    }
    if (!_slots.ContainsKey(destination.Id))
    {
      throw new ArgumentException($"The Pokémon '{destination}' was not found in trainer's 'Id={TrainerId}' storage.", nameof(destination));
    }

    if ((source.IsEggInBox && destination.IsHatchedInParty) || (destination.IsEggInBox && source.IsHatchedInParty))
    {
      EnsurePartyIsNotEmpty(party, [source.Id, destination.Id]);
    }

    source.Swap(destination, actorId);
    Raise(new StoredPokemonSwapped(source.Id, destination.Id), actorId);
  }
  protected virtual void Handle(StoredPokemonSwapped @event)
  {
    PokemonSlot sourceSlot = _slots[@event.SourceId];
    PokemonSlot destinationSlot = _slots[@event.DestinationId];

    _pokemon[sourceSlot] = @event.DestinationId;
    _pokemon[destinationSlot] = @event.SourceId;

    _slots[@event.SourceId] = destinationSlot;
    _slots[@event.DestinationId] = sourceSlot;
  }

  public void Withdraw(Specimen pokemon, ActorId? actorId = null)
  {
    if (!_slots.TryGetValue(pokemon.Id, out PokemonSlot? previousSlot))
    {
      throw new ArgumentException($"The Pokémon '{pokemon}' was not found in trainer's 'Id={TrainerId}' storage.", nameof(pokemon));
    }
    else if (previousSlot.Box is null)
    {
      throw new ArgumentException($"The Pokémon '{pokemon}' is already in trainer's 'Id={TrainerId}' party.", nameof(pokemon));
    }

    Position? position = FindFirstPartyAvailable();
    if (position is null)
    {
      ValidationFailure failure = new(nameof(TrainerId), "The specified trainer party is full.", TrainerId.ToGuid())
      {
        CustomState = new
        {
          Party = GetParty().Select(id => id.ToGuid()).ToArray()
        },
        ErrorCode = "TrainerPartyIsFull"
      };
      throw new ValidationException([failure]);
    }

    pokemon.Withdraw(position, actorId);
    Raise(new PokemonStored(pokemon.Id, new PokemonSlot(position)), actorId);
  }

  private void EnsurePartyIsNotEmpty(
    IReadOnlyDictionary<PokemonId, Specimen> party,
    IReadOnlyCollection<PokemonId>? excludedIds = null,
    IReadOnlyCollection<PokemonId>? partyIds = null)
  {
    excludedIds ??= [];
    partyIds ??= GetParty();

    if (!partyIds.Any(id => !excludedIds.Contains(id) && !party[id].IsEgg))
    {
      ValidationFailure failure = new(nameof(TrainerId), "The operation would leave the trainer party empty of non-egg Pokémon.", TrainerId.ToGuid())
      {
        CustomState = new
        {
          Party = GetParty().Select(id => id.ToGuid()).ToArray()
        },
        ErrorCode = "NotEmptyPartyValidator"
      };
      throw new ValidationException([failure]);
    }
  }

  private PokemonSlot FindFirstAvailable()
  {
    Position? position = FindFirstPartyAvailable();
    return position is null ? FindFirstBoxAvailable() : new PokemonSlot(position);
  }
  private Position? FindFirstPartyAvailable()
  {
    IReadOnlyCollection<PokemonId> party = GetParty();
    return party.Count >= PokemonSlot.PartySize ? null : new Position(party.Count);
  }
  private PokemonSlot FindFirstBoxAvailable()
  {
    for (int boxNumber = 0; boxNumber < PokemonSlot.BoxCount; boxNumber++)
    {
      Box box = new(boxNumber);
      for (int positionNumber = 0; positionNumber < PokemonSlot.BoxSize; positionNumber++)
      {
        Position position = new(positionNumber);
        PokemonSlot slot = new(position, box);
        if (!_pokemon.ContainsKey(slot))
        {
          return slot;
        }
      }
    }

    ValidationFailure failure = new(nameof(TrainerId), "The specified trainer storage is full.", TrainerId.ToGuid())
    {
      ErrorCode = "TrainerStorageIsFull"
    };
    throw new ValidationException([failure]);
  }

  private void ShiftPartyMembers(
    PokemonSlot previousSlot,
    IReadOnlyDictionary<PokemonId, Specimen> party,
    IReadOnlyCollection<PokemonId>? excludedIds = null,
    IReadOnlyCollection<PokemonId>? partyIds = null,
    ActorId? actorId = null)
  {
    excludedIds ??= [];
    partyIds ??= GetParty();

    foreach (PokemonId partyId in partyIds)
    {
      if (!excludedIds.Contains(partyId))
      {
        Specimen member = party[partyId];
        PokemonSlot slot = _slots[partyId];
        if (slot.IsGreaterThan(previousSlot))
        {
          slot = slot.Previous();
          member.Move(slot, actorId);
          Raise(new PokemonStored(partyId, slot), actorId);
        }
      }
    }
  }
}
