using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Trainers.Events;

namespace PokeGame.Core.Trainers;

public class Trainer : AggregateRoot
{
  private TrainerUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null || _updated.Description is not null
    || _updated.Gender is not null || _updated.Money is not null || _updated.UserId is not null
    || _updated.Sprite is not null || _updated.Url is not null || _updated.Notes is not null;

  public new TrainerId Id => new(base.Id);

  private License? _license = null;
  public License License => _license ?? throw new InvalidOperationException("The trainer has not been initialized.");

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The trainer has not been initialized.");
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

  private TrainerGender? _gender;
  public TrainerGender Gender
  {
    get => _gender ?? throw new InvalidOperationException("The trainer has not been initialized.");
    set
    {
      if (_gender != value)
      {
        _gender = value;
        _updated.Gender = new Change<TrainerGender>(value);
      }
    }
  }
  private Money? _money = null;
  public Money Money
  {
    get => _money ?? throw new InvalidOperationException("The trainer has not been initialized.");
    set
    {
      if (_money != value)
      {
        _money = value;
        _updated.Money = new Change<Money>(value);
      }
    }
  }

  private Guid? _userId = null;
  public Guid? UserId
  {
    get => _userId;
    set
    {
      if (_userId != value)
      {
        _userId = value;
        _updated.UserId = new Change<Guid?>(value);
      }
    }
  }

  private Url? _sprite = null;
  public Url? Sprite
  {
    get => _sprite;
    set
    {
      if (_sprite != value)
      {
        _sprite = value;
        _updated.Sprite = new Change<Url>(value);
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

  public Trainer() : base()
  {
  }

  public Trainer(License license, UniqueName uniqueName, TrainerGender gender = TrainerGender.Male, Money? money = null, ActorId? actorId = null, TrainerId? trainerId = null)
    : base((trainerId ?? TrainerId.NewId()).StreamId)
  {
    if (!Enum.IsDefined(gender))
    {
      throw new ArgumentOutOfRangeException(nameof(gender));
    }

    money ??= new();
    Raise(new TrainerCreated(license, uniqueName, gender, money), actorId);
  }
  protected virtual void Handle(TrainerCreated @event)
  {
    _license = @event.License;

    _uniqueName = @event.UniqueName;

    _gender = @event.Gender;
    _money = @event.Money;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new TrainerDeleted(), actorId);
    }
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new TrainerUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(TrainerUniqueNameChanged @event)
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
  protected virtual void Handle(TrainerUpdated @event)
  {
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Gender is not null)
    {
      _gender = @event.Gender.Value;
    }
    if (@event.Money is not null)
    {
      _money = @event.Money.Value;
    }

    if (@event.UserId is not null)
    {
      _userId = @event.UserId.Value;
    }

    if (@event.Sprite is not null)
    {
      _sprite = @event.Sprite.Value;
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
