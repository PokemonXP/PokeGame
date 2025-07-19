using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon;

public class Specimen : AggregateRoot
{
  public const int MoveLimit = 4;

  private PokemonUpdated _updated = new();
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

  public EggCycles? EggCycles { get; private set; }
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

  private readonly Dictionary<MoveId, PokemonMove> _learnedMoves = [];
  public IReadOnlyDictionary<MoveId, PokemonMove> LearnedMoves => _learnedMoves.AsReadOnly();
  private readonly List<MoveId> _currentMoves = new(capacity: MoveLimit);
  public IReadOnlyCollection<KeyValuePair<MoveId, PokemonMove>> CurrentMoves
    => _currentMoves.Select(id => new KeyValuePair<MoveId, PokemonMove>(id, _learnedMoves[id])).ToList().AsReadOnly();

  public Specimen() : base()
  {
  }

  public Specimen(
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
    EggCycles? eggCycles = null,
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
    if (eggCycles is not null)
    {
      if (eggCycles.Value > species.EggCycles.Value)
      {
        throw new ArgumentOutOfRangeException(nameof(eggCycles));
      }
      if (experience > 0)
      {
        throw new ArgumentException("An egg Pokémon cannot have experience.", nameof(experience));
      }
    }

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

    PokemonCreated created = new(species.Id, variety.Id, form.Id, uniqueName, gender, isShiny, teraType.Value, size, abilitySlot, nature,
      species.GrowthRate, eggCycles, experience, form.BaseStatistics, individualValues, effortValues, vitality.Value, stamina.Value, friendship);
    Raise(created, actorId);
  }
  protected virtual void Handle(PokemonCreated @event)
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

    EggCycles = @event.EggCycles;
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

  public void HoldItem(Item item, ActorId? actorId = null) => HoldItem(item.Id, actorId);
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

  public bool LearnMove(Move move, int? position = null, Level? level = null, Notes? notes = null, ActorId? actorId = null)
  {
    if (_learnedMoves.ContainsKey(move.Id))
    {
      return false;
    }

    if (position.HasValue)
    {
      ArgumentOutOfRangeException.ThrowIfNegative(position.Value, nameof(position));
      ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position.Value, MoveLimit, nameof(position));
    }
    if (_currentMoves.Count < MoveLimit)
    {
      position = _currentMoves.Count;
    }

    byte powerPoints = move.PowerPoints.Value;
    MoveLearningMethod method = level is null ? MoveLearningMethod.Evolving : MoveLearningMethod.LevelingUp;
    level ??= new(Level);
    PokemonMove pokemonMove = new(powerPoints, powerPoints, move.PowerPoints, IsMastered: false, level, method, ItemId: null, notes);
    Raise(new PokemonMoveLearned(move.Id, pokemonMove, position), actorId);

    return true;
  }
  protected virtual void Handle(PokemonMoveLearned @event)
  {
    _learnedMoves[@event.MoveId] = @event.Move;

    if (@event.Position.HasValue)
    {
      if (@event.Position.Value == _currentMoves.Count)
      {
        _currentMoves.Add(@event.MoveId);
      }
      else
      {
        _currentMoves[@event.Position.Value] = @event.MoveId;
      }
    }
  }

  public bool RelearnMove(Move move, int position, ActorId? actorId = null) => RelearnMove(move.Id, position, actorId);
  public bool RelearnMove(MoveId moveId, int position, ActorId? actorId = null)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(position, nameof(position));
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position, MoveLimit, nameof(position));

    if (!_learnedMoves.ContainsKey(moveId) || _currentMoves.Contains(moveId))
    {
      return false;
    }

    Raise(new PokemonMoveRelearned(moveId, position), actorId);
    return true;
  }
  protected virtual void Handle(PokemonMoveRelearned @event)
  {
    _currentMoves[@event.Position] = @event.MoveId;
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

  public void SetNickname(Nickname? nickname, ActorId? actorId = null)
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

  public override string ToString() => $"{Nickname?.Value ?? UniqueName.Value} | {base.ToString()}";
}

/* Ownership Kinds:
 * Caught
 * Hatched
 * Bought
 * (Gifted/Received?)
 * Traded
 * (Winned?)
 */

/* Ownership Properties:
 * OriginalTrainerId: TrainerId
 * CurrentTrainerId: TrainerId
 * Kind: OwnershipKind
 * MetAtLevel: Int32
 * MetLocation: String
 * MetOn: DateTime
 * (flavor_text)
 * Notes?: Notes
 * Position: Byte
 * Box: Byte?
 */
