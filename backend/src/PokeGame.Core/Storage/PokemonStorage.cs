using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Storage.Events;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Storage;

public class PokemonStorage : AggregateRoot
{
  public new PokemonStorageId Id => new(base.Id);
  public TrainerId TrainerId => Id.TrainerId;

  private readonly Dictionary<PokemonId, PokemonSlot> _slots = [];
  public IReadOnlyDictionary<PokemonId, PokemonSlot> Slots => _slots.AsReadOnly();

  private readonly Dictionary<PokemonSlot, PokemonId> _pokemon = [];
  public IReadOnlyDictionary<PokemonSlot, PokemonId> Pokemon => _pokemon.AsReadOnly();

  private readonly List<PokemonId> _party = [];
  public IReadOnlyCollection<PokemonId> Party => _party.AsReadOnly();

  private readonly Dictionary<Box, Dictionary<Position, PokemonId>> _boxes = [];
  public IReadOnlyDictionary<Box, IReadOnlyDictionary<Position, PokemonId>> Boxes
  {
    get
    {
      Dictionary<Box, IReadOnlyDictionary<Position, PokemonId>> boxes = new(capacity: _boxes.Count);
      foreach (var box in _boxes)
      {
        boxes[box.Key] = box.Value.ToDictionary(x => x.Key, x => x.Value).AsReadOnly();
      }
      return boxes.AsReadOnly();
    }
  }

  public PokemonStorage() : base()
  {
  }

  public PokemonStorage(Trainer trainer) : base(new PokemonStorageId(trainer.Id).StreamId)
  {
  }

  public void Add(Specimen pokemon, ActorId? actorId = null)
  {
    if (pokemon.Ownership is null || pokemon.Ownership.TrainerId != TrainerId)
    {
      throw new ArgumentException($"The Pokémon current trainer 'Id={pokemon.Ownership?.TrainerId.Value ?? "<null>"}' must be 'Id={TrainerId}'.", nameof(pokemon));
    }
    else if (!_slots.ContainsKey(pokemon.Id))
    {
      PokemonSlot slot = FindFirstAvailable();
      Raise(new PokemonStored(pokemon.Id, slot), actorId);
    }
  }
  protected virtual void Handle(PokemonStored @event)
  {
    _slots[@event.PokemonId] = @event.Slot;
    _pokemon[@event.Slot] = @event.PokemonId;

    if (@event.Slot.Box is null)
    {
      _party.Add(@event.PokemonId);
    }
  }

  public void Deposit(Specimen pokemon, IEnumerable<Specimen> party, ActorId? actorId = null)
  {
    if (pokemon.Ownership is null || pokemon.Ownership.TrainerId != TrainerId)
    {
      throw new ArgumentException($"The Pokémon current trainer 'Id={pokemon.Ownership?.TrainerId.Value ?? "<null>"}' must be 'Id={TrainerId}'.", nameof(pokemon));
    }

    PokemonSlot existingSlot = _slots[pokemon.Id];
    if (existingSlot.Box is not null)
    {
      return;
    }

    if (!pokemon.IsEgg)
    {
      // TODO(fpion): s’assurer qu’il y a au moins un autre Pokémon non-œuf dans le groupe.
    }

    // TODO(fpion): décaler la position (-1) des autres Pokémon du party après le Pokémon déposé.

    // TODO(fpion): raise event
  }

  private PokemonSlot FindFirstAvailable()
  {
    var position = FindPartyFirstAvailable();
    return position is null ? FindBoxFirstAvailable() : new PokemonSlot(position);
  }
  private Position? FindPartyFirstAvailable()
  {
    var count = _party.Count;
    return count < PokemonSlot.PartySize ? new Position(count) : null;
  }
  private PokemonSlot FindBoxFirstAvailable()
  {
    for (var boxNumber = 0; boxNumber < Box.Count; boxNumber++)
    {
      Box box = new(boxNumber);
      if (!_boxes.TryGetValue(box, out var positions))
      {
        return new PokemonSlot(new Position(0), box);
      }

      for (var positionNumber = 0; positionNumber < Box.Size; positionNumber++)
      {
        Position position = new(positionNumber);
        if (!positions.ContainsKey(position))
        {
          return new PokemonSlot(position, box);
        }
      }
    }

    throw new PokemonStorageFullException(this);
  }
}
