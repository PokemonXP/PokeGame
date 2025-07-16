using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Speciez;
using PokeGame.Core.Varieties;
using PokemonSpecies = PokeGame.Core.Speciez.Species;

namespace PokeGame.Core.Pokemons;

public class Pokemon2 : AggregateRoot
{
  private PokemonUpdated2 _updated = new();
  private bool HasUpdates => _updated.Sprite is not null || _updated.Url is not null || _updated.Notes is not null;

  public new PokemonId Id => new(base.Id);

  public SpeciesId SpeciesId { get; private set; }
  public VarietyId VarietyId { get; private set; }
  public FormId FormId { get; private set; }

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  public Nickname? Nickname { get; private set; }
  public PokemonGender? Gender { get; private set; }
  public bool IsShiny { get; private set; }

  public PokemonType TeraType { get; private set; }
  private PokemonSize? _size = null;
  public PokemonSize Size => _size ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  public AbilitySlot AbilitySlot { get; private set; }
  private PokemonNature? _nature = null;
  public PokemonNature Nature => _nature ?? throw new InvalidOperationException("The Pokémon has not been initialized.");

  public GrowthRate GrowthRate { get; private set; }
  public int Experience { get; private set; }
  public int Level => ExperienceTable.Instance.GetLevel(GrowthRate, Experience);

  private BaseStatistics? _baseStatistics = null;
  public BaseStatistics BaseStatistics => _baseStatistics ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  private IndividualValues? _individualValues = null;
  public IndividualValues IndividualValues => _individualValues ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  private EffortValues? _effortValues = null;
  public EffortValues EffortValues => _effortValues ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  public PokemonStatistics Statistics => new(this);
  public int Vitality { get; private set; }
  public int Stamina { get; private set; }
  public StatusCondition? StatusCondition { get; private set; }
  private Friendship? _friendship = null;
  public Friendship Friendship => _friendship ?? throw new InvalidOperationException("The Pokémon has not been initialized.");

  public PokemonCharacteristic Characteristic => PokemonCharacteristics.Instance.Find(IndividualValues, Size);

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

  public Pokemon2() : base()
  {
  }

  public Pokemon2(
    PokemonSpecies species,
    Variety variety,
    Form form,
    UniqueName uniqueName,
    PokemonSize size,
    PokemonNature nature,
    IndividualValues individualValues,
    PokemonGender? gender = null,
    bool isShiny = false,
    PokemonType? teraType = null,
    AbilitySlot abilitySlot = AbilitySlot.Primary,
    int experience = 0,
    EffortValues? effortValues = null,
    int? vitality = null,
    int? stamina = null,
    Friendship? friendship = null,
    ActorId? actorId = null,
    PokemonId? pokemonId = null) : base((pokemonId ?? PokemonId.NewId()).StreamId)
  {
    if (variety.SpeciesId != species.Id)
    {
      throw new ArgumentException($"The variety '{variety}' does not belong to the species '{species}'.", nameof(variety));
    }

    if (form.VarietyId != variety.Id)
    {
      throw new ArgumentException($"The form '{form}' does not belong to the variety '{variety}'.", nameof(form));
    }

    if (gender.HasValue && !Enum.IsDefined(gender.Value))
    {
      throw new ArgumentOutOfRangeException(nameof(gender));
    }
    if (variety.GenderRatio is null)
    {
      if (gender.HasValue)
      {
        throw new ArgumentException("The Pokémon should not have a gender (unknown).", nameof(gender));
      }
    }
    else
    {
      if (!gender.HasValue)
      {
        throw new ArgumentNullException(nameof(gender), "The Pokémon should have a gender.");
      }
      else if (!variety.GenderRatio.IsValid(gender.Value))
      {
        throw new ArgumentOutOfRangeException(nameof(gender), $"The gender '{gender}' is not valid for ratio '{variety.GenderRatio}'.");
      }
    }

    if (teraType.HasValue && !Enum.IsDefined(teraType.Value))
    {
      throw new ArgumentOutOfRangeException(nameof(teraType));
    }
    teraType ??= form.Types.Primary;

    if (!Enum.IsDefined(abilitySlot))
    {
      throw new ArgumentOutOfRangeException(nameof(abilitySlot));
    }
    if ((abilitySlot == AbilitySlot.Secondary && form.Abilities.Secondary is null) || (abilitySlot == AbilitySlot.Hidden && form.Abilities.Hidden is null))
    {
      throw new ArgumentException($"The ability slot cannot be '{abilitySlot}' because the form does not have an ability for this slot.", nameof(abilitySlot));
    }

    ArgumentOutOfRangeException.ThrowIfNegative(experience, nameof(experience));

    effortValues ??= new();

    int level = ExperienceTable.Instance.GetLevel(species.GrowthRate, experience);
    PokemonStatistics statistics = new(form.BaseStatistics, individualValues, effortValues, level, nature);

    if (vitality.HasValue)
    {
      ArgumentOutOfRangeException.ThrowIfNegative(vitality.Value, nameof(vitality));
    }
    if (vitality is null || vitality > statistics.HP)
    {
      vitality = statistics.HP;
    }

    if (stamina.HasValue)
    {
      ArgumentOutOfRangeException.ThrowIfNegative(stamina.Value, nameof(stamina));
    }
    if (stamina is null || stamina > statistics.HP)
    {
      stamina = statistics.HP;
    }

    friendship ??= species.BaseFriendship;

    PokemonCreated2 created = new(species.Id, variety.Id, form.Id, uniqueName, gender, isShiny, teraType.Value, size, abilitySlot, nature,
      species.GrowthRate, experience, form.BaseStatistics, individualValues, effortValues, vitality.Value, stamina.Value, friendship);
    Raise(created, actorId);
  }
  protected virtual void Handle(PokemonCreated2 @event)
  {
    SpeciesId = @event.SpeciesId;
    VarietyId = @event.VarietyId;
    FormId = @event.FormId;

    _uniqueName = @event.UniqueName;
    Gender = @event.Gender;
    IsShiny = @event.IsShiny;

    TeraType = @event.TeraType;
    _size = @event.Size;
    AbilitySlot = @event.AbilitySlot;
    _nature = @event.Nature;

    GrowthRate = @event.GrowthRate;
    Experience = @event.Experience;

    _baseStatistics = @event.BaseStatistics;
    _individualValues = @event.IndividualValues;
    _effortValues = @event.EffortValues;
    Vitality = @event.Vitality;
    Stamina = @event.Stamina;
    _friendship = @event.Friendship;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new PokemonDeleted(), actorId);
    }
  }

  public void SetNickname(Nickname? nickname, ActorId? actorId = null)
  {
    if (Nickname != nickname)
    {
      Raise(new PokemonNicknamed2(nickname), actorId);
    }
  }
  protected virtual void Handle(PokemonNicknamed2 @event)
  {
    Nickname = @event.Nickname;
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new PokemonUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(PokemonUniqueNameChanged @event)
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

  public override string ToString() => $"{Nickname?.Value ?? UniqueName.Value} | {base.ToString()}";
}
