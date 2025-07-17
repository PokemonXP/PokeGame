using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties.Events;

namespace PokeGame.Core.Varieties;

public class Variety : AggregateRoot
{
  private VarietyUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null
    || _updated.Genus is not null || _updated.Description is not null
    || _updated.GenderRatio is not null || _updated.CanChangeForm.HasValue
    || _updated.Url is not null || _updated.Notes is not null;

  public new VarietyId Id => new(base.Id);

  public SpeciesId SpeciesId { get; private set; }
  public bool IsDefault { get; private set; }

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The variety has not been initialized.");
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

  private Genus? _genus = null;
  public Genus? Genus
  {
    get => _genus;
    set
    {
      if (_genus != value)
      {
        _genus = value;
        _updated.Genus = new Change<Genus>(value);
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

  private GenderRatio? _genderRatio = null;
  public GenderRatio? GenderRatio
  {
    get => _genderRatio;
    set
    {
      if (_genderRatio != value)
      {
        _genderRatio = value;
        _updated.GenderRatio = new Change<GenderRatio>(value);
      }
    }
  }

  private bool? _canChangeForm = null;
  public bool CanChangeForm
  {
    get => _canChangeForm ?? throw new InvalidOperationException("The variety has not been initialized.");
    set
    {
      if (_canChangeForm != value)
      {
        _canChangeForm = value;
        _updated.CanChangeForm = value;
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

  public Variety() : base()
  {
  }

  public Variety(
    PokemonSpecies species,
    UniqueName uniqueName,
    bool isDefault = false,
    GenderRatio? genderRatio = null,
    bool canChangeForm = false,
    ActorId? actorId = null,
    VarietyId? varietyId = null) : base((varietyId ?? VarietyId.NewId()).StreamId)
  {
    Raise(new VarietyCreated(species.Id, isDefault, uniqueName, genderRatio, canChangeForm), actorId);
  }
  protected virtual void Handle(VarietyCreated @event)
  {
    SpeciesId = @event.SpeciesId;
    IsDefault = @event.IsDefault;

    _uniqueName = @event.UniqueName;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new VarietyDeleted(), actorId);
    }
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new VarietyUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(VarietyUniqueNameChanged @event)
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
  protected virtual void Handle(VarietyUpdated @event)
  {
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }

    if (@event.Genus is not null)
    {
      _genus = @event.Genus.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }

    if (@event.GenderRatio is not null)
    {
      _genderRatio = @event.GenderRatio.Value;
    }

    if (@event.CanChangeForm.HasValue)
    {
      _canChangeForm = @event.CanChangeForm.Value;
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
