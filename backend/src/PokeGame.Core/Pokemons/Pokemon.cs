using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemons;

public class Pokemon : AggregateRoot
{
  public const int MoveLimit = 4;

  private PokemonUpdated _updated = new();
  private bool HasUpdates => _updated.Gender is not null
    || _updated.Vitality.HasValue || _updated.Stamina.HasValue || _updated.StatusCondition is not null
    || _updated.Friendship.HasValue
    || _updated.Sprite is not null || _updated.Url is not null || _updated.Notes is not null;

  public new PokemonId Id => new(base.Id);

  public FormId FormId { get; private set; }

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

  public PokemonType TeraType { get; private set; }
  private PokemonSize? _size = null;
  public PokemonSize Size => _size ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  public AbilitySlot AbilitySlot { get; private set; }
  private PokemonNature? _nature = null;
  public PokemonNature Nature => _nature ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
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
  private int _vitality = 0;
  public int Vitality
  {
    get => _vitality;
    set
    {
      int vitality = Math.Min(value, Statistics.HP);
      ArgumentOutOfRangeException.ThrowIfNegative(vitality, nameof(Vitality));

      if (_vitality != vitality)
      {
        _vitality = vitality;
        _updated.Vitality = vitality;
      }
    }
  }
  private int _stamina = 0;
  public int Stamina
  {
    get => _stamina;
    set
    {
      int stamina = Math.Min(value, Statistics.HP);
      ArgumentOutOfRangeException.ThrowIfNegative(stamina, nameof(Stamina));

      if (_stamina != stamina)
      {
        _stamina = stamina;
        _updated.Stamina = stamina;
      }
    }
  }
  private StatusCondition? _statusCondition = null;
  public StatusCondition? StatusCondition
  {
    get => _statusCondition;
    set
    {
      if (_statusCondition != value)
      {
        _statusCondition = value;
        _updated.StatusCondition = new Change<StatusCondition?>(value);
      }
    }
  }
  private PokemonCharacteristic? _characteristic = null;
  public PokemonCharacteristic Characteristic => _characteristic ?? throw new InvalidOperationException("The Pokémon has not been initialized.");

  private byte _friendship = 0;
  public byte Friendship
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

  public ItemId? HeldItemId { get; private set; }

  private readonly Dictionary<MoveId, PokemonMove> _allMoves = [];
  public IReadOnlyDictionary<MoveId, PokemonMove> AllMoves => _allMoves.AsReadOnly();
  private readonly List<MoveId> _moves = new(capacity: MoveLimit);
  public IReadOnlyCollection<PokemonMove> Moves => _moves.Select(id => _allMoves[id]).ToList().AsReadOnly();

  public TrainerId? OriginalTrainerId { get; private set; }
  public PokemonOwnership? Ownership { get; private set; }
  public bool IsTraded => OriginalTrainerId.HasValue && OriginalTrainerId.Value != Ownership?.TrainerId;

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
    _vitality = @event.Vitality;
    _stamina = @event.Stamina;
    _characteristic = @event.Characteristic;

    _friendship = @event.Friendship;
  }

  public bool Catch(TrainerId trainerId, ItemId pokeBallId, GameLocation location, DateTime? caughtOn = null, Description? description = null, ActorId? actorId = null)
  {
    if (Ownership is not null)
    {
      return false;
    }

    Raise(new PokemonCaught(trainerId, pokeBallId, Level, location, description), actorId, caughtOn);
    return true;
  }
  protected virtual void Handle(PokemonCaught @event)
  {
    OriginalTrainerId = @event.TrainerId;
    Ownership = new PokemonOwnership(OwnershipKind.Caught, @event.TrainerId, @event.PokeBallId, @event.Level, @event.Location, @event.OccurredOn, @event.Description);
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new PokemonDeleted(), actorId);
    }
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

  public bool LearnMove(MoveId moveId, PowerPoints powerPoints, ActorId? actorId = null) => LearnMove(moveId, powerPoints, position: null, actorId);
  public bool LearnMove(MoveId moveId, PowerPoints powerPoints, int? position, ActorId? actorId = null)
  {
    if (position < 0 || position >= MoveLimit)
    {
      throw new ArgumentOutOfRangeException(nameof(position));
    }
    if (position is null && _moves.Count == MoveLimit)
    {
      throw new ArgumentNullException(nameof(position));
    }
    if (_allMoves.ContainsKey(moveId))
    {
      return false;
    }

    position = _moves.Count < MoveLimit ? null : position;
    Raise(new PokemonMoveLearned(moveId, powerPoints, position), actorId);

    return true;
  }
  protected virtual void Handle(PokemonMoveLearned @event)
  {
    PokemonMove move = new(@event.MoveId, @event.PowerPoints.Value, @event.PowerPoints.Value, @event.PowerPoints, IsMastered: false, Level, TechnicalMachine: false);
    SetMove(move, @event.Position);
  }

  public bool MasterMove(MoveId moveId, ActorId? actorId = null)
  {
    if (!_allMoves.TryGetValue(moveId, out PokemonMove? move))
    {
      return false;
    }
    else if (!move.IsMastered)
    {
      Raise(new PokemonMoveMastered(moveId), actorId);
    }

    return true;
  }
  protected virtual void Handle(PokemonMoveMastered @event)
  {
    _allMoves[@event.MoveId] = _allMoves[@event.MoveId].Master();
  }

  public void Receive(TrainerId trainerId, ItemId pokeBallId, GameLocation location, DateTime? receivedOn = null, Description? description = null, ActorId? actorId = null)
  {
    if (Ownership?.TrainerId != trainerId)
    {
      pokeBallId = Ownership?.PokeBallId ?? pokeBallId;
      Raise(new PokemonReceived(trainerId, pokeBallId, Level, location, description), actorId, receivedOn);
    }
  }
  protected virtual void Handle(PokemonReceived @event)
  {
    if (!OriginalTrainerId.HasValue)
    {
      OriginalTrainerId = @event.TrainerId;
    }

    Ownership = new PokemonOwnership(OwnershipKind.Received, @event.TrainerId, @event.PokeBallId, @event.Level, @event.Location, @event.OccurredOn, @event.Description);
  }

  public bool RelearnMove(MoveId moveId, int position, ActorId? actorId = null)
  {
    if (position < 0 || position >= MoveLimit)
    {
      throw new ArgumentOutOfRangeException(nameof(position));
    }
    else if (!_allMoves.ContainsKey(moveId) || _moves.Contains(moveId))
    {
      return false;
    }

    Raise(new PokemonMoveRelearned(moveId, position), actorId);
    return true;
  }
  protected virtual void Handle(PokemonMoveRelearned @event)
  {
    _moves[@event.Position] = @event.MoveId;
  }

  public void Release(ActorId? actorId = null)
  {
    if (OriginalTrainerId.HasValue || Ownership is not null)
    {
      Raise(new PokemonReleased(), actorId);
    }
  }
  protected virtual void Handle(PokemonReleased _)
  {
    OriginalTrainerId = null;
    Ownership = null;
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

  public void SwitchMoves(int source, int destination, ActorId? actorId = null)
  {
    if (source < 0 || source >= MoveLimit)
    {
      throw new ArgumentOutOfRangeException(nameof(source));
    }
    if (destination < 0 || destination >= MoveLimit)
    {
      throw new ArgumentOutOfRangeException(nameof(destination));
    }

    if (source != destination && source < _moves.Count && destination < _moves.Count)
    {
      Raise(new PokemonMovesSwitched(source, destination), actorId);
    }
  }
  protected virtual void Handle(PokemonMovesSwitched @event)
  {
    MoveId source = _moves[@event.Source];
    MoveId destination = _moves[@event.Destination];

    _moves[@event.Source] = destination;
    _moves[@event.Destination] = source;
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

    if (@event.Vitality.HasValue)
    {
      _vitality = @event.Vitality.Value;
    }
    if (@event.Stamina.HasValue)
    {
      _stamina = @event.Stamina.Value;
    }
    if (@event.StatusCondition is not null)
    {
      _statusCondition = @event.StatusCondition.Value;
    }

    if (@event.Friendship.HasValue)
    {
      _friendship = @event.Friendship.Value;
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

  public bool UseTechnicalMachine(MoveId moveId, PowerPoints powerPoints, ActorId? actorId = null) => UseTechnicalMachine(moveId, powerPoints, position: null, actorId);
  public bool UseTechnicalMachine(MoveId moveId, PowerPoints powerPoints, int? position, ActorId? actorId = null)
  {
    if (position < 0 || position >= MoveLimit)
    {
      throw new ArgumentOutOfRangeException(nameof(position));
    }
    if (position is null && _moves.Count == MoveLimit)
    {
      throw new ArgumentNullException(nameof(position));
    }
    if (_allMoves.ContainsKey(moveId))
    {
      return false;
    }

    position = _moves.Count < MoveLimit ? null : position;
    Raise(new PokemonTechnicalMachineUsed(moveId, powerPoints, position), actorId);

    return true;
  }
  protected virtual void Handle(PokemonTechnicalMachineUsed @event)
  {
    PokemonMove move = new(@event.MoveId, @event.PowerPoints.Value, @event.PowerPoints.Value, @event.PowerPoints, IsMastered: false, Level, TechnicalMachine: true);
    SetMove(move, @event.Position);
  }

  private void SetMove(PokemonMove move, int? position)
  {
    _allMoves[move.MoveId] = move;

    if (position.HasValue)
    {
      _moves[position.Value] = move.MoveId;
    }
    else
    {
      _moves.Add(move.MoveId);
    }
  }

  public override string ToString() => $"{Nickname?.Value ?? UniqueName.Value} | {base.ToString()}";
}
