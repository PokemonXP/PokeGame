using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Storage.Events;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Storage;

public class PokemonStorage : AggregateRoot
{
  public const int BoxCount = 32;
  public const int BoxSize = 5 * 6;
  public const int PartySize = 6;

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
    if (!_slots.ContainsKey(pokemon.Id))
    {
      throw new ArgumentException($"The Pokémon '{pokemon}' was not found in trainer's 'Id={TrainerId}' storage.", nameof(pokemon));
    }

    #region TODO(fpion): refactor
    bool isValid = GetParty().Any(id => id != pokemon.Id && !party[id].IsEgg);
    if (!isValid)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
    #endregion

    // TODO(fpion): move-up every party Pokémon before deposited Pokémon!!!

    PokemonSlot slot = FindFirstBoxAvailable();
    pokemon.Deposit(slot, actorId);
    Raise(new PokemonStored(pokemon.Id, slot), actorId);
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
      #region TODO(fpion): refactor
      bool isValid = GetParty().Any(id => id != source.Id && id != destination.Id && !party[id].IsEgg);
      if (!isValid)
      {
        throw new NotImplementedException(); // TODO(fpion): implement
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

  private PokemonSlot FindFirstAvailable()
  {
    Position? position = FindFirstPartyAvailable();
    return position is null ? FindFirstBoxAvailable() : new PokemonSlot(position);
  }
  private Position? FindFirstPartyAvailable()
  {
    IReadOnlyCollection<PokemonId> party = GetParty();
    return party.Count >= PartySize ? null : new Position(party.Count);
  }
  private PokemonSlot FindFirstBoxAvailable()
  {
    for (int boxNumber = 0; boxNumber < BoxCount; boxNumber++)
    {
      Box box = new(boxNumber);
      for (int positionNumber = 0; positionNumber < BoxSize; positionNumber++)
      {
        Position position = new(positionNumber);
        PokemonSlot slot = new(position, box);
        if (!_pokemon.ContainsKey(slot))
        {
          return slot;
        }
      }
    }
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
