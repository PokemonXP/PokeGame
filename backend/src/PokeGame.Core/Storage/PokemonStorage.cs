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

    #region TASK: [POKEGAME-263](https://logitar.atlassian.net/browse/POKEGAME-263)
    bool isValid = partyIds.Any(id => id != pokemon.Id && !party[id].IsEgg);
    if (!isValid)
    {
      throw new NotImplementedException(); // TASK: [POKEGAME-263](https://logitar.atlassian.net/browse/POKEGAME-263)
    }
    #endregion

    PokemonSlot newSlot = FindFirstBoxAvailable();
    pokemon.Deposit(newSlot, actorId);
    Raise(new PokemonStored(pokemon.Id, newSlot));

    foreach (PokemonId partyId in partyIds)
    {
      if (partyId != pokemon.Id)
      {
        PokemonSlot slot = _slots[partyId];
        if (slot.IsGreaterThan(previousSlot))
        {
          Raise(new PokemonStored(partyId, slot.Previous()), actorId);
        }
      }
    }
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
      #region TASK: [POKEGAME-263](https://logitar.atlassian.net/browse/POKEGAME-263)
      bool isValid = GetParty().Any(id => id != source.Id && id != destination.Id && !party[id].IsEgg);
      if (!isValid)
      {
        throw new NotImplementedException(); // TASK: [POKEGAME-263](https://logitar.atlassian.net/browse/POKEGAME-263)
      }
      #endregion
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

    Position position = FindFirstPartyAvailable() ?? throw new NotImplementedException(); // TASK: [POKEGAME-263](https://logitar.atlassian.net/browse/POKEGAME-263)
    pokemon.Withdraw(position, actorId);
    Raise(new PokemonStored(pokemon.Id, new PokemonSlot(position)), actorId);
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
    throw new NotImplementedException(); // TASK: [POKEGAME-263](https://logitar.atlassian.net/browse/POKEGAME-263)
  }
}
