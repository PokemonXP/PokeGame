using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms.Events;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Forms;

public class Form : AggregateRoot
{
  private FormUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null || _updated.Description is not null
    || _updated.IsBattleOnly.HasValue || _updated.IsMega.HasValue
    || _updated.Height is not null || _updated.Weight is not null
    || _updated.Types is not null || _updated.Abilities is not null || _updated.BaseStatistics is not null || _updated.Yield is not null || _updated.Sprites is not null
    || _updated.Url is not null || _updated.Notes is not null;

  public new FormId Id => new(base.Id);

  public VarietyId VarietyId { get; private set; }
  public bool IsDefault { get; private set; }

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The form has not been initialized.");
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

  private bool? _isBattleOnly = null;
  public bool IsBattleOnly
  {
    get => _isBattleOnly ?? throw new InvalidOperationException("The form has not been initialized.");
    set
    {
      if (_isBattleOnly != value)
      {
        _isBattleOnly = value;
        _updated.IsBattleOnly = value;
      }
    }
  }
  private bool? _isMega = null;
  public bool IsMega
  {
    get => _isMega ?? throw new InvalidOperationException("The form has not been initialized.");
    set
    {
      if (_isMega != value)
      {
        _isMega = value;
        _updated.IsMega = value;
      }
    }
  }

  private Height? _height = null;
  public Height Height
  {
    get => _height ?? throw new InvalidOperationException("The form has not been initialized.");
    set
    {
      if (_height != value)
      {
        _height = value;
        _updated.Height = value;
      }
    }
  }
  private Weight? _weight = null;
  public Weight Weight
  {
    get => _weight ?? throw new InvalidOperationException("The form has not been initialized.");
    set
    {
      if (_weight != value)
      {
        _weight = value;
        _updated.Weight = value;
      }
    }
  }

  private FormTypes? _types = null;
  public FormTypes Types
  {
    get => _types ?? throw new InvalidOperationException("The form has not been initialized.");
    set
    {
      if (_types != value)
      {
        _types = value;
        _updated.Types = value;
      }
    }
  }
  private FormAbilities? _abilities = null;
  public FormAbilities Abilities
  {
    get => _abilities ?? throw new InvalidOperationException("The form has not been initialized.");
    set
    {
      if (_abilities != value)
      {
        _abilities = value;
        _updated.Abilities = value;
      }
    }
  }
  private BaseStatistics? _baseStatistics = null;
  public BaseStatistics BaseStatistics
  {
    get => _baseStatistics ?? throw new InvalidOperationException("The form has not been initialized.");
    set
    {
      if (_baseStatistics != value)
      {
        _baseStatistics = value;
        _updated.BaseStatistics = value;
      }
    }
  }
  private Yield? _yield = null;
  public Yield Yield
  {
    get => _yield ?? throw new InvalidOperationException("The form has not been initialized.");
    set
    {
      if (_yield != value)
      {
        _yield = value;
        _updated.Yield = value;
      }
    }
  }
  private Sprites? _sprites = null;
  public Sprites Sprites
  {
    get => _sprites ?? throw new InvalidOperationException("The form has not been initialized.");
    set
    {
      if (_sprites != value)
      {
        _sprites = value;
        _updated.Sprites = value;
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

  public Form() : base()
  {
  }

  public Form(
    Variety variety,
    UniqueName uniqueName,
    FormTypes types,
    FormAbilities abilities,
    BaseStatistics baseStatistics,
    Yield yield,
    Sprites sprites,
    bool isDefault = false,
    bool isBattleOnly = false,
    bool isMega = false,
    Height? height = null,
    Weight? weight = null,
    ActorId? actorId = null,
    FormId? formId = null) : base((formId ?? FormId.NewId()).StreamId)
  {
    height ??= new(1);
    weight ??= new(1);
    Raise(new FormCreated(variety.Id, isDefault, uniqueName, isBattleOnly, isMega, height, weight, types, abilities, baseStatistics, yield, sprites), actorId);
  }
  protected virtual void Handle(FormCreated @event)
  {
    VarietyId = @event.VarietyId;
    IsDefault = @event.IsDefault;

    _uniqueName = @event.UniqueName;

    _isBattleOnly = @event.IsBattleOnly;
    _isMega = @event.IsMega;

    _height = @event.Height;
    _weight = @event.Weight;

    _types = @event.Types;
    _abilities = @event.Abilities;
    _baseStatistics = @event.BaseStatistics;
    _yield = @event.Yield;
    _sprites = @event.Sprites;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new FormDeleted(), actorId);
    }
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new FormUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(FormUniqueNameChanged @event)
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
  protected virtual void Handle(FormUpdated @event)
  {
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }

    if (@event.IsBattleOnly.HasValue)
    {
      _isBattleOnly = @event.IsBattleOnly.Value;
    }
    if (@event.IsMega.HasValue)
    {
      _isMega = @event.IsMega.Value;
    }

    if (@event.Height is not null)
    {
      _height = @event.Height;
    }
    if (@event.Weight is not null)
    {
      _weight = @event.Weight;
    }

    if (@event.Types is not null)
    {
      _types = @event.Types;
    }
    if (@event.Abilities is not null)
    {
      _abilities = @event.Abilities;
    }
    if (@event.BaseStatistics is not null)
    {
      _baseStatistics = @event.BaseStatistics;
    }
    if (@event.Yield is not null)
    {
      _yield = @event.Yield;
    }
    if (@event.Sprites is not null)
    {
      _sprites = @event.Sprites;
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
