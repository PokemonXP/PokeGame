using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Moves.Events;

namespace PokeGame.Core.Moves;

public class Move : AggregateRoot
{
  private MoveUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null || _updated.Description is not null
    || _updated.Accuracy is not null || _updated.Power is not null || _updated.PowerPoints is not null
    || _updated.Url is not null || _updated.Notes is not null;

  public new MoveId Id => new(base.Id);

  public PokemonType Type { get; private set; }
  public MoveCategory Category { get; private set; }

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The move has not been initialized.");
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

  private Accuracy? _accuracy = null;
  public Accuracy? Accuracy
  {
    get => _accuracy;
    set
    {
      if (_accuracy != value)
      {
        _accuracy = value;
        _updated.Accuracy = new Change<Accuracy>(value);
      }
    }
  }
  private Power? _power = null;
  public Power? Power
  {
    get => _power;
    set
    {
      if (Category == MoveCategory.Status && value is not null)
      {
        throw new ArgumentException("A status move cannot have power.", nameof(Power));
      }

      if (_power != value)
      {
        _power = value;
        _updated.Power = new Change<Power>(value);
      }
    }
  }
  private PowerPoints? _powerPoints = null;
  public PowerPoints PowerPoints
  {
    get => _powerPoints ?? throw new InvalidOperationException("The move has not been initialized.");
    set
    {
      if (_powerPoints != value)
      {
        _powerPoints = value;
        _updated.PowerPoints = value;
      }
    }
  }

  private Url? _url = null;
  public Url? Url
  {
    get => _url;
    set
    {
      if (_url != value)
      {
        _url = value;
        _updated.Url = new Change<Url>(value);
      }
    }
  }
  private Notes? _notes = null;
  public Notes? Notes
  {
    get => _notes;
    set
    {
      if (_notes != value)
      {
        _notes = value;
        _updated.Notes = new Change<Notes>(value);
      }
    }
  }

  public Move() : base()
  {
  }

  public Move(
    PokemonType type,
    MoveCategory category,
    UniqueName uniqueName,
    Accuracy? accuracy = null,
    Power? power = null,
    PowerPoints? powerPoints = null,
    ActorId? actorId = null,
    MoveId? moveId = null) : base((moveId ?? MoveId.NewId()).StreamId)
  {
    if (!Enum.IsDefined(type))
    {
      throw new ArgumentOutOfRangeException(nameof(type));
    }
    if (!Enum.IsDefined(category))
    {
      throw new ArgumentOutOfRangeException(nameof(category));
    }

    if (category == MoveCategory.Status && power is not null)
    {
      throw new ArgumentException("A status move cannot have power.", nameof(power));
    }

    powerPoints ??= new(1);
    Raise(new MoveCreated(type, category, uniqueName, accuracy, power, powerPoints), actorId);
  }
  protected virtual void Handle(MoveCreated @event)
  {
    Type = @event.Type;
    Category = @event.Category;

    _uniqueName = @event.UniqueName;

    _accuracy = @event.Accuracy;
    _power = @event.Power;
    _powerPoints = @event.PowerPoints;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new MoveDeleted(), actorId);
    }
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new MoveUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(MoveUniqueNameChanged @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(MoveUpdated @event)
  {
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Accuracy is not null)
    {
      _accuracy = @event.Accuracy.Value;
    }
    if (@event.Power is not null)
    {
      _power = @event.Power.Value;
    }
    if (@event.PowerPoints is not null)
    {
      _powerPoints = @event.PowerPoints;
    }

    if (@event.Url is not null)
    {
      _url = @event.Url.Value;
    }
    if (@event.Notes is not null)
    {
      _notes = @event.Notes.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
