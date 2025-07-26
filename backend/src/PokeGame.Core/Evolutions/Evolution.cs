using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Evolutions.Events;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;

namespace PokeGame.Core.Evolutions;

public class Evolution : AggregateRoot
{
  private EvolutionUpdated _updated = new();
  public bool HasUpdates => _updated.Level is not null || _updated.Friendship.HasValue || _updated.Gender is not null
    || _updated.HeldItemId is not null || _updated.KnownMoveId is not null || _updated.Location is not null || _updated.TimeOfDay is not null;

  public new EvolutionId Id => new(base.Id);

  public FormId SourceId { get; private set; }
  public FormId TargetId { get; private set; }

  public EvolutionTrigger Trigger { get; private set; }
  public ItemId? ItemId { get; private set; }

  private Level? _level = null;
  public Level? Level
  {
    get => _level;
    set
    {
      if (_level != value)
      {
        _level = value;
        _updated.Level = new Change<Level>(value);
      }
    }
  }
  private bool _friendship = false;
  public bool Friendship
  {
    get => _friendship;
    set
    {
      if (_friendship != value)
      {
        _friendship = value;
        _updated.Friendship = value;
      }
    }
  }
  private PokemonGender? _gender = null;
  public PokemonGender? Gender
  {
    get => _gender;
    set
    {
      if (_gender != value)
      {
        _gender = value;
        _updated.Gender = new Change<PokemonGender?>(value);
      }
    }
  }
  private ItemId? _heldItemId = null;
  public ItemId? HeldItemId
  {
    get => _heldItemId;
    set
    {
      if (_heldItemId != value)
      {
        _heldItemId = value;
        _updated.HeldItemId = new Change<ItemId?>(value);
      }
    }
  }
  private MoveId? _knownMoveId = null;
  public MoveId? KnownMoveId
  {
    get => _knownMoveId;
    set
    {
      if (_knownMoveId != value)
      {
        _knownMoveId = value;
        _updated.KnownMoveId = new Change<MoveId?>(value);
      }
    }
  }
  private Location? _location = null;
  public Location? Location
  {
    get => _location;
    set
    {
      if (_location != value)
      {
        _location = value;
        _updated.Location = new Change<Location>(value);
      }
    }
  }
  private TimeOfDay? _timeOfDay = null;
  public TimeOfDay? TimeOfDay
  {
    get => _timeOfDay;
    set
    {
      if (_timeOfDay != value)
      {
        _timeOfDay = value;
        _updated.TimeOfDay = new Change<TimeOfDay?>(value);
      }
    }
  }

  public Evolution() : base()
  {
  }

  public Evolution(Form source, Form target, EvolutionTrigger trigger = EvolutionTrigger.Level, Item? item = null, ActorId? actorId = null, EvolutionId? evolutionId = null)
    : base((evolutionId ?? EvolutionId.NewId()).StreamId)
  {
    if (source.Equals(target) || source.VarietyId == target.VarietyId)
    {
      throw new ArgumentException("The source and target forms must be different, and be of a different variety.", nameof(target));
    }
    if (!Enum.IsDefined(trigger))
    {
      throw new ArgumentOutOfRangeException(nameof(trigger));
    }

    if (trigger == EvolutionTrigger.Item && item is null)
    {
      throw new ArgumentNullException(nameof(item), $"The item is required when the trigger is '{EvolutionTrigger.Item}'.");
    }
    else if (trigger != EvolutionTrigger.Item && item is not null)
    {
      throw new ArgumentException($"The item must be null when the trigger is not '{EvolutionTrigger.Item}'.", nameof(item));
    }

    Raise(new EvolutionCreated(source.Id, target.Id, trigger, item?.Id), actorId);
  }
  protected virtual void Handle(EvolutionCreated @event)
  {
    SourceId = @event.SourceId;
    TargetId = @event.TargetId;

    Trigger = @event.Trigger;
    ItemId = @event.ItemId;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new EvolutionDeleted(), actorId);
    }
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(EvolutionUpdated @event)
  {
    if (@event.Level is not null)
    {
      _level = @event.Level.Value;
    }
    if (@event.Friendship.HasValue)
    {
      _friendship = @event.Friendship.Value;
    }
    if (@event.Gender is not null)
    {
      _gender = @event.Gender.Value;
    }
    if (@event.HeldItemId is not null)
    {
      _heldItemId = @event.HeldItemId.Value;
    }
    if (@event.KnownMoveId is not null)
    {
      _knownMoveId = @event.KnownMoveId.Value;
    }
    if (@event.Location is not null)
    {
      _location = @event.Location.Value;
    }
    if (@event.TimeOfDay is not null)
    {
      _timeOfDay = @event.TimeOfDay.Value;
    }
  }
}
