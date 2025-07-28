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

  private readonly Dictionary<PokemonId, PokemonSlot> _slots = [];
  public IReadOnlyDictionary<PokemonId, PokemonSlot> Slots => _slots.AsReadOnly();

  private readonly Dictionary<PokemonSlot, PokemonId> _pokemon = [];
  public IReadOnlyDictionary<PokemonSlot, PokemonId> Pokemon => _pokemon.AsReadOnly();

  public PokemonStorage() : base()
  {
  }

  public PokemonStorage(Trainer trainer) : base(new PokemonStorageId(trainer.Id).StreamId)
  {
  }

  protected virtual void Handle(PokemonStored @event)
  {
    if (_slots.TryGetValue(@event.PokemonId, out PokemonSlot? previousSlot))
    {
      _pokemon.Remove(previousSlot);
    }

    _slots[@event.PokemonId] = @event.Slot;
    _pokemon[@event.Slot] = @event.PokemonId;
  }

  public void Add(Specimen pokemon, ActorId? actorId = null)
  {
    ValidateOwnership(pokemon);

    if (!_slots.ContainsKey(pokemon.Id))
    {
      PokemonSlot slot = FindFirstAvailable();
      pokemon.Move(slot, actorId);
      Raise(new PokemonStored(pokemon.Id, slot), actorId);
    }
  }

  public void Deposit(Specimen pokemon, IReadOnlyDictionary<PokemonId, Specimen> party, ActorId? actorId = null)
  {
    ValidateOwnership(pokemon);

    PokemonSlot existingSlot = _slots[pokemon.Id];
    if (existingSlot.Box is not null)
    {
      return;
    }

    IReadOnlyCollection<PokemonId> partyIds = GetParty();
    if (!pokemon.IsEgg && !partyIds.Any(id => id != pokemon.Id && !party[id].IsEgg))
    {
      ValidationFailure failure = new("PokemonId", "The trainer Pokémon party must contain at least another non-egg Pokémon.", pokemon.Id.ToGuid())
      {
        ErrorCode = "NonEmptyPartyValidator"
      };
      throw new ValidationException([failure]);
    }

    PokemonSlot newSlot = FindBoxFirstAvailable();

    foreach (PokemonId memberId in partyIds)
    {
      Specimen member = party[memberId];
      if (!member.Equals(pokemon))
      {
        PokemonSlot memberSlot = _slots[member.Id];
        if (memberSlot.Position.Value > existingSlot.Position.Value)
        {
          memberSlot = new PokemonSlot(new Position(memberSlot.Position.Value - 1));
          member.Move(memberSlot, actorId);
          Raise(new PokemonStored(member.Id, memberSlot), actorId);
        }
      }
    }

    pokemon.Deposit(newSlot, actorId);
    Raise(new PokemonStored(pokemon.Id, newSlot), actorId);
  }

  public IReadOnlyDictionary<Box, IReadOnlyDictionary<Position, PokemonId>> GetBoxes()
  {
    Dictionary<Box, Dictionary<Position, PokemonId>> boxes = new(capacity: Box.Count);
    foreach (KeyValuePair<PokemonSlot, PokemonId> pokemon in _pokemon)
    {
      if (pokemon.Key.Box is not null)
      {
        if (!boxes.TryGetValue(pokemon.Key.Box, out Dictionary<Position, PokemonId>? box))
        {
          box = [];
          boxes[pokemon.Key.Box] = box;
        }
        box[pokemon.Key.Position] = pokemon.Value;
      }
    }
    return boxes.ToDictionary(x => x.Key, x => (IReadOnlyDictionary<Position, PokemonId>)x.Value.AsReadOnly()).AsReadOnly();
  }
  public IReadOnlyCollection<PokemonId> GetParty() => _pokemon
    .Where(x => x.Key.Box is null)
    .OrderBy(x => x.Key.Position.Value)
    .Select(x => x.Value).ToList().AsReadOnly();

  private PokemonSlot FindFirstAvailable()
  {
    Position? position = FindPartyFirstAvailable();
    return position is null ? FindBoxFirstAvailable() : new PokemonSlot(position);
  }
  private Position? FindPartyFirstAvailable()
  {
    int count = GetParty().Count;
    return count < PokemonSlot.PartySize ? new Position(count) : null;
  }
  private PokemonSlot FindBoxFirstAvailable()
  {
    IReadOnlyDictionary<Box, IReadOnlyDictionary<Position, PokemonId>> boxes = GetBoxes();

    for (int boxNumber = 0; boxNumber < Box.Count; boxNumber++)
    {
      Box box = new(boxNumber);
      if (!boxes.TryGetValue(box, out IReadOnlyDictionary<Position, PokemonId>? positions))
      {
        return new PokemonSlot(new Position(0), box);
      }

      for (int positionNumber = 0; positionNumber < Box.Size; positionNumber++)
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

  private void ValidateOwnership(Specimen pokemon)
  {
    if (pokemon.Ownership is null || pokemon.Ownership.TrainerId != TrainerId)
    {
      ValidationFailure failure = new("PokemonId", $"The Pokémon current trainer must be 'Id={TrainerId.ToGuid()}'.", pokemon.Id.ToGuid())
      {
        CustomState = new { TrainerId = pokemon.Ownership?.TrainerId.ToGuid() },
        ErrorCode = "OwnershipValidator",
      };
      throw new ValidationException([failure]);
    }
  }
}
