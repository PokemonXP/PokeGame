using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;

namespace PokeGame.Core.Pokemons;

public class Pokemon : AggregateRoot
{
  private PokemonUpdated _updated = new();
  private bool HasUpdates => _updated.Gender is not null || _updated.Url is not null || _updated.Notes is not null;

  public new PokemonId Id => new(base.Id);

  public FormId FormId { get; private set; } // TODO(fpion): can be changed (change form / evolve)!

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  public DisplayName? Nickname { get; private set; }
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

  public PokemonType TeraType { get; private set; } // TODO(fpion): can be changed using shards!
  private PokemonSize? _size = null;
  public PokemonSize Size => _size ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  public AbilitySlot AbilitySlot { get; private set; } // TODO(fpion): can be changed using Ability Patch/Capsule!
  private PokemonNature? _nature = null;
  public PokemonNature Nature => _nature ?? throw new InvalidOperationException("The Pokémon has not been initialized."); // TODO(fpion): can be changed using mints!
  public Flavor? FavoriteFlavor => Nature.FavoriteFlavor;
  public Flavor? DislikedFlavor => Nature.DislikedFlavor;

  public GrowthRate GrowthRate { get; private set; }
  public int Experience { get; private set; }
  public int Level => ExperienceTable.Instance.GetLevel(GrowthRate, Experience);
  public int MaximumExperience => ExperienceTable.Instance.GetMaximumExperience(GrowthRate, Level);
  public int ToNextLevel => Math.Max(MaximumExperience - Experience, 0);

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
  private PokemonCharacteristic? _characteristic = null;
  public PokemonCharacteristic Characteristic => _characteristic ?? throw new InvalidOperationException("The Pokémon has not been initialized.");

  public byte Friendship { get; private set; }

  public ItemId? HeldItemId { get; private set; }

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
  private Description? _notes = null;
  public Description? Notes
  {
    get => _notes;
    set
    {
      if (_notes != value)
      {
        _notes = value;
        _updated.Notes = new Change<Description>(value);
      }
    }
  }

  public Pokemon() : base()
  {
  }

  public Pokemon(
    FormId formId,
    UniqueName uniqueName,
    PokemonType teraType,
    PokemonSize size,
    PokemonNature nature,
    BaseStatistics baseStatistics,
    PokemonGender? gender = null,
    AbilitySlot abilitySlot = AbilitySlot.Primary,
    IndividualValues? individualValues = null,
    EffortValues? effortValues = null,
    GrowthRate growthRate = default,
    int experience = 0,
    int vitality = 0,
    int stamina = 0,
    byte friendship = 0,
    ActorId? actorId = null,
    PokemonId? pokemonId = null) : base((pokemonId ?? PokemonId.NewId()).StreamId)
  {
    if (!Enum.IsDefined(teraType))
    {
      throw new ArgumentOutOfRangeException(nameof(teraType));
    }
    if (gender.HasValue && !Enum.IsDefined(gender.Value))
    {
      throw new ArgumentOutOfRangeException(nameof(gender));
    }
    if (!Enum.IsDefined(abilitySlot))
    {
      throw new ArgumentOutOfRangeException(nameof(abilitySlot));
    }
    if (!Enum.IsDefined(growthRate))
    {
      throw new ArgumentOutOfRangeException(nameof(growthRate));
    }
    ArgumentOutOfRangeException.ThrowIfNegative(experience, nameof(experience));
    ArgumentOutOfRangeException.ThrowIfNegative(vitality, nameof(vitality));
    ArgumentOutOfRangeException.ThrowIfNegative(stamina, nameof(stamina));

    int level = ExperienceTable.Instance.GetLevel(growthRate, experience);
    individualValues ??= new();
    effortValues ??= new();
    PokemonStatistics statistics = new(baseStatistics, individualValues, effortValues, level, nature);
    vitality = Math.Min(vitality, statistics.HP);
    stamina = Math.Min(stamina, statistics.HP);
    PokemonCharacteristic characteristic = PokemonCharacteristics.Instance.Find(individualValues, size);

    Raise(new PokemonCreated(
      formId,
      uniqueName,
      gender,
      teraType,
      size,
      abilitySlot,
      nature,
      growthRate,
      experience,
      baseStatistics,
      individualValues,
      effortValues,
      vitality,
      stamina,
      characteristic,
      friendship), actorId);
  }
  protected virtual void Handle(PokemonCreated @event)
  {
    FormId = @event.FormId;

    _uniqueName = @event.UniqueName;
    _gender = @event.Gender;

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
    _characteristic = @event.Characteristic;

    Friendship = @event.Friendship;
  }

  public void HoldItem(ItemId itemId, ActorId? actorId = null)
  {
    if (HeldItemId != itemId)
    {
      Raise(new PokemonItemHeld(itemId), actorId);
    }
  }
  protected virtual void Handle(PokemonItemHeld @event)
  {
    HeldItemId = @event.ItemId;
  }

  public void RemoveItem(ActorId? actorId = null)
  {
    if (HeldItemId.HasValue)
    {
      Raise(new PokemonItemRemoved(), actorId);
    }
  }
  protected virtual void Handle(PokemonItemRemoved _)
  {
    HeldItemId = null;
  }

  public void SetNickname(DisplayName? nickname, ActorId? actorId = null)
  {
    if (Nickname != nickname)
    {
      Raise(new PokemonNicknamed(nickname), actorId);
    }
  }
  protected virtual void Handle(PokemonNicknamed @event)
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
  protected virtual void Handle(PokemonUpdated @event)
  {
    if (@event.Gender is not null)
    {
      _gender = @event.Gender.Value;
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

  public override string ToString() => $"{Nickname?.Value ?? UniqueName.Value} | {base.ToString()}";

  // TODO(fpion): OriginalTrainer / PokéBall / Met(Level + Location + Date + TextOverride)
  // TODO(fpion): CurrentTrainer / PokéBall / Met(Level + Location + Date + TextOverride)
}

/* TODO(fpion): Moves
 * Id
 * CurrentPP
 * MaximumPP
 * ReferencePP
 * Mastered
 * Position
 * Relearn
 * Reorder
 * LearnedAtLevel
 * LearnedFromTM
 */
