using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Regions;
using PokeGame.Core.Species.Events;

namespace PokeGame.Core.Species;

public class PokemonSpecies : AggregateRoot
{
  private SpeciesUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null
    || _updated.BaseFriendship is not null || _updated.CatchRate is not null || _updated.GrowthRate.HasValue
    || _updated.EggGroups is not null
    || _updated.Url is not null || _updated.Notes is not null;

  public new SpeciesId Id => new(base.Id);

  private Number? _number = null;
  public Number Number => _number ?? throw new InvalidOperationException("The species has not been initialized.");
  public PokemonCategory Category { get; private set; }

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The species has not been initialized.");
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

  private Friendship? _baseFriendship = null;
  public Friendship BaseFriendship
  {
    get => _baseFriendship ?? throw new InvalidOperationException("The species has not been initialized.");
    set
    {
      if (_baseFriendship != value)
      {
        _baseFriendship = value;
        _updated.BaseFriendship = value;
      }
    }
  }
  private CatchRate? _catchRate = null;
  public CatchRate CatchRate
  {
    get => _catchRate ?? throw new InvalidOperationException("The species has not been initialized.");
    set
    {
      if (_catchRate != value)
      {
        _catchRate = value;
        _updated.CatchRate = value;
      }
    }
  }
  private GrowthRate? _growthRate = null;
  public GrowthRate GrowthRate
  {
    get => _growthRate ?? throw new InvalidOperationException("The species has not been initialized.");
    set
    {
      if (_growthRate != value)
      {
        _growthRate = value;
        _updated.GrowthRate = value;
      }
    }
  }

  private EggGroups? _eggGroups = null;
  public EggGroups EggGroups
  {
    get => _eggGroups ?? throw new InvalidOperationException("The species has not been initialized.");
    set
    {
      if (_eggGroups != value)
      {
        _eggGroups = value;
        _updated.EggGroups = value;
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

  private readonly Dictionary<RegionId, Number> _regionalNumbers = [];
  public IReadOnlyDictionary<RegionId, Number> RegionalNumbers => _regionalNumbers.AsReadOnly();

  public PokemonSpecies() : base()
  {
  }

  public PokemonSpecies(
    Number number,
    PokemonCategory category,
    UniqueName uniqueName,
    Friendship? baseFriendship = null,
    CatchRate? catchRate = null,
    GrowthRate growthRate = GrowthRate.MediumFast,
    EggGroups? eggGroups = null,
    ActorId? actorId = null,
    SpeciesId? speciesId = null) : base((speciesId ?? SpeciesId.NewId()).StreamId)
  {
    if (!Enum.IsDefined(category))
    {
      throw new ArgumentOutOfRangeException(nameof(category));
    }
    if (!Enum.IsDefined(growthRate))
    {
      throw new ArgumentOutOfRangeException(nameof(growthRate));
    }

    baseFriendship ??= new Friendship();
    catchRate ??= new CatchRate(1);
    eggGroups ??= new EggGroups();
    Raise(new SpeciesCreated(number, category, uniqueName, baseFriendship, catchRate, growthRate, eggGroups), actorId);
  }
  protected virtual void Handle(SpeciesCreated @event)
  {
    _number = @event.Number;
    Category = @event.Category;

    _uniqueName = @event.UniqueName;

    _baseFriendship = @event.BaseFriendship;
    _catchRate = @event.CatchRate;
    _growthRate = @event.GrowthRate;

    _eggGroups = @event.EggGroups;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new SpeciesDeleted(), actorId);
    }
  }

  public void SetRegionalNumber(Region region, Number? number, ActorId? actorId = null) => SetRegionalNumber(region.Id, number, actorId);
  public void SetRegionalNumber(RegionId regionId, Number? number, ActorId? actorId = null)
  {
    _regionalNumbers.TryGetValue(regionId, out Number? existingNumber);
    if (existingNumber != number)
    {
      Raise(new SpeciesRegionalNumberChanged(regionId, number), actorId);
    }
  }
  protected virtual void Handle(SpeciesRegionalNumberChanged @event)
  {
    if (@event.Number is null)
    {
      _regionalNumbers.Remove(@event.RegionId);
    }
    else
    {
      _regionalNumbers[@event.RegionId] = @event.Number;
    }
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new SpeciesUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(SpeciesUniqueNameChanged @event)
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
  protected virtual void Handle(SpeciesUpdated @event)
  {
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }

    if (@event.BaseFriendship is not null)
    {
      _baseFriendship = @event.BaseFriendship;
    }
    if (@event.CatchRate is not null)
    {
      _catchRate = @event.CatchRate;
    }
    if (@event.GrowthRate.HasValue)
    {
      _growthRate = @event.GrowthRate.Value;
    }

    if (@event.EggGroups is not null)
    {
      _eggGroups = @event.EggGroups;
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
