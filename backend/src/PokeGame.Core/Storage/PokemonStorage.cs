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

  protected virtual void Handle(PokemonStored @event)
  {
    _pokemon[@event.Slot] = @event.PokemonId; // TODO(fpion): won't work when stored is used to move a Pokémon
    _slots[@event.PokemonId] = @event.Slot;
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

  private IReadOnlyCollection<PokemonId> GetParty() => _slots
    .Where(x => x.Value.Box is null)
    .OrderBy(x => x.Value.Position.Value)
    .Select(x => x.Key).ToList().AsReadOnly();
}
