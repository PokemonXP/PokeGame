using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core;
using Logitar;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon;

public class Specimen : AggregateRoot
{
  public const int MoveLimit = 4;
  public const byte EvolutionFriendship = 200;

  private PokemonUpdated _updated = new();
  private bool HasUpdates => _updated.IsShiny.HasValue
    || _updated.Vitality.HasValue || _updated.Stamina.HasValue || _updated.StatusCondition is not null || _updated.Friendship is not null
    || _updated.Sprite is not null || _updated.Url is not null || _updated.Notes is not null;

  public new PokemonId Id => new(base.Id);

  public SpeciesId SpeciesId { get; private set; }
  public VarietyId VarietyId { get; private set; }
  public FormId FormId { get; private set; }

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  public Nickname? Nickname { get; private set; }
  public PokemonGender? Gender { get; private set; }
  private bool _isShiny = false;
  public bool IsShiny
  {
    get => _isShiny;
    set
    {
      if (_isShiny != value)
      {
        _isShiny = value;
        _updated.IsShiny = value;
      }
    }
  }

  public PokemonType TeraType { get; private set; }
  private PokemonSize? _size = null;
  public PokemonSize Size => _size ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
  public AbilitySlot AbilitySlot { get; private set; }
  private PokemonNature? _nature = null;
  public PokemonNature Nature => _nature ?? throw new InvalidOperationException("The Pokémon has not been initialized.");

  public EggCycles? EggCycles { get; private set; }
  public bool IsEgg => EggCycles is not null;
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
  private int _vitality = 0;
  public int Vitality
  {
    get => _vitality;
    set
    {
      ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(Vitality));

      int vitality = Math.Min(value, Statistics.HP);
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
      ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(Stamina));

      int stamina = Math.Min(value, Statistics.HP);
      if (_stamina != value)
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
  private Friendship? _friendship = null;
  public Friendship Friendship
  {
    get => _friendship ?? throw new InvalidOperationException("The Pokémon has not been initialized.");
    set
    {
      if (_friendship != value)
      {
        _friendship = value;
        _updated.Friendship = value;
      }
    }
  }

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

  public TrainerId? OriginalTrainerId { get; private set; }
  public Ownership? Ownership { get; private set; }
  public PokemonSlot? Slot { get; private set; }

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
    _isShiny = @event.IsShiny;

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
    _vitality = @event.Vitality;
    _stamina = @event.Stamina;
    _friendship = @event.Friendship;
  }

  public void Catch(Trainer trainer, Item pokeBall, Location location, Level? level = null, DateTime? metOn = null, Description? description = null, PokemonSlot? slot = null, ActorId? actorId = null)
  {
    if (Ownership is not null)
    {
      throw new TrainerPokemonCannotBeCaughtException(this);
    }

    SetOwnership(OwnershipKind.Caught, trainer, pokeBall, location, level, metOn, description, slot, actorId);

    PokeBallProperties properties = (PokeBallProperties)pokeBall.Properties;
    if (properties.Heal)
    {
      Heal(actorId);
    }
    if (properties.BaseFriendship > Friendship.Value)
    {
      Friendship = new Friendship(properties.BaseFriendship);
      Update(actorId);
    }
  }

  public void ChangeForm(Form form, ActorId? actorId = null)
  {
    if (form.VarietyId != VarietyId)
    {
      throw new ArgumentException("The Pokémon current and target forms must be from the same variety.", nameof(form));
    }
    else if (FormId != form.Id)
    {

      Raise(new PokemonFormChanged(form.Id, form.BaseStatistics), actorId);
    }
  }
  protected virtual void Handle(PokemonFormChanged @event)
  {
    FormId = @event.FormId;

    _baseStatistics = @event.BaseStatistics;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new PokemonDeleted(), actorId);
    }
  }

  public void Deposit(PokemonSlot slot, ActorId? actorId = null)
  {
    if (Ownership is null)
    {
      throw new InvalidOperationException($"The Pokémon 'Id={Id}' is not owned by any trainer.");
    }
    else if (slot.Box is null)
    {
      throw new ArgumentException("The Pokémon must be deposited inside a box.", nameof(slot));
    }
    else if (Slot != slot)
    {
      Raise(new PokemonDeposited(slot), actorId);
    }
  }
  protected virtual void Handle(PokemonDeposited @event)
  {
    Slot = @event.Slot;
  }

  public void Evolve(PokemonSpecies species, Variety variety, Form form, Evolution evolution, ActorId? actorId = null)
  {
    List<ValidationFailure> failures = new(capacity: 10);

    if (variety.SpeciesId != species.Id)
    {
      failures.Add(new ValidationFailure(nameof(variety), "The variety does not belong to the species.", variety.Id)
      {
        CustomState = new { SpeciesId = species.Id },
        ErrorCode = "InvalidVariety"
      });
    }
    if (form.VarietyId != variety.Id)
    {
      failures.Add(new ValidationFailure(nameof(form), "The form does not belong to the variety.", form.Id)
      {
        CustomState = new { VarietyId = variety.Id },
        ErrorCode = "InvalidForm"
      });
    }

    if (FormId != evolution.SourceId)
    {
      failures.Add(new ValidationFailure("EvolutionId", "The Pokémon form should be the evolution source form.", evolution.Id.ToGuid())
      {
        CustomState = new { SourceId = evolution.SourceId.ToGuid() },
        ErrorCode = "InvalidSourceForm"
      });
    }
    if (form.Id != evolution.TargetId)
    {
      failures.Add(new ValidationFailure(nameof(form), "The form is not the target evolution form.", form.Id)
      {
        CustomState = new { evolution.TargetId },
        ErrorCode = "InvalidTargetForm"
      });
    }

    if (evolution.Trigger == EvolutionTrigger.Trade && (!OriginalTrainerId.HasValue || Ownership is null || OriginalTrainerId == Ownership.TrainerId))
    {
      failures.Add(new ValidationFailure("PokemonId", "The Pokémon current and original trainers must be different for traded evolutions.", Id.ToGuid())
      {
        ErrorCode = "PokemonNotTraded"
      });
    }

    if (evolution.Level is not null && Level < evolution.Level.Value)
    {
      failures.Add(new ValidationFailure(nameof(Level), $"The Pokémon level must be greater than or equal to {evolution.Level}.", Level)
      {
        ErrorCode = "LevelRequirementNotMet"
      });
    }
    if (evolution.Friendship && Friendship.Value < EvolutionFriendship)
    {
      failures.Add(new ValidationFailure(nameof(Friendship), $"The Pokémon friendship must be greater than or equal to {EvolutionFriendship}.", Friendship.Value)
      {
        ErrorCode = "FriendshipRequirementNotMet"
      });
    }
    if (evolution.Gender.HasValue && evolution.Gender.Value != Gender)
    {
      failures.Add(new ValidationFailure(nameof(Gender), $"The Pokémon gender must be '{evolution.Gender}'.", Gender)
      {
        ErrorCode = "GenderRequirementNotMet"
      });
    }
    if (evolution.HeldItemId.HasValue && evolution.HeldItemId.Value != HeldItemId)
    {
      failures.Add(new ValidationFailure(nameof(HeldItemId), $"The Pokémon must hold the item 'Id={evolution.HeldItemId.Value.ToGuid()}'.", HeldItemId?.ToGuid())
      {
        ErrorCode = "HeldItemRequirementNotMet"
      });
    }
    if (evolution.KnownMoveId.HasValue && !LearnedMoves.ContainsKey(evolution.KnownMoveId.Value))
    {
      failures.Add(new ValidationFailure(nameof(CurrentMoves), $"The Pokémon must currently know the move 'Id={evolution.KnownMoveId.Value.ToGuid()}'.", CurrentMoves.Select(move => move.Key.ToGuid()))
      {
        ErrorCode = "KnownMoveRequirementNotMet"
      });
    }

    if (failures.Count > 0)
    {
      throw new ValidationException(failures);
    }

    Raise(new PokemonEvolved(species.Id, variety.Id, form.Id, form.BaseStatistics), actorId);
  }
  protected virtual void Handle(PokemonEvolved @event)
  {
    SpeciesId = @event.SpeciesId;
    VarietyId = @event.VarietyId;
    FormId = @event.FormId;

    _baseStatistics = @event.BaseStatistics;
  }

  public void Heal(ActorId? actorId = null)
  {
    int constitution = Statistics.HP;
    if (Vitality != constitution || Stamina != constitution || StatusCondition.HasValue || LearnedMoves.Values.Any(move => move.CurrentPowerPoints < move.MaximumPowerPoints))
    {
      Raise(new PokemonHealed(), actorId);
    }
  }
  protected virtual void Handle(PokemonHealed _)
  {
    int constitution = Statistics.HP;
    _vitality = constitution;
    _stamina = constitution;
    _statusCondition = null;

    List<KeyValuePair<MoveId, PokemonMove>> moves = _learnedMoves.ToList();
    foreach (KeyValuePair<MoveId, PokemonMove> move in moves)
    {
      _learnedMoves[move.Key] = move.Value.RestorePowerPoints();
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

  public bool LearnMove(
    Move move,
    int? position = null,
    Level? level = null,
    MoveLearningMethod method = MoveLearningMethod.LevelingUp,
    Notes? notes = null,
    ActorId? actorId = null)
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

  public void Move(PokemonSlot slot, ActorId? actorId = null)
  {
    if (Ownership is null)
    {
      throw new InvalidOperationException($"The Pokémon 'Id={Id}' is not owned by any trainer.");
    }
    else if (Slot != slot)
    {
      Raise(new PokemonMoved(slot), actorId);
    }
  }
  protected virtual void Handle(PokemonMoved @event)
  {
    Slot = @event.Slot;
  }

  public void Receive(Trainer trainer, Item pokeBall, Location location, Level? level = null, DateTime? metOn = null, Description? description = null, PokemonSlot? slot = null, ActorId? actorId = null)
  {
    SetOwnership(OwnershipKind.Received, trainer, pokeBall, location, level, metOn, description, slot, actorId);
  }

  public void Release(ActorId? actorId = null)
  {
    if (Ownership is not null)
    {
      if (Slot?.Box is null)
      {
        throw new CannotReleasePartyPokemonException(this);
      }

      Raise(new PokemonReleased(), actorId);
    }
  }
  protected virtual void Handle(PokemonReleased _)
  {
    OriginalTrainerId = null;
    Ownership = null;
    Slot = null;
  }

  public bool RememberMove(Move move, int position, ActorId? actorId = null) => RememberMove(move.Id, position, actorId);
  public bool RememberMove(MoveId moveId, int position, ActorId? actorId = null)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(position, nameof(position));
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position, MoveLimit, nameof(position));

    if (!_learnedMoves.ContainsKey(moveId))
    {
      return false;
    }
    else if (!_currentMoves.Contains(moveId))
    {
      Raise(new PokemonMoveRemembered(moveId, position), actorId);
    }

    return true;
  }
  protected virtual void Handle(PokemonMoveRemembered @event)
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

  public void Swap(Specimen pokemon, ActorId? actorId = null)
  {
    if (Ownership is null || Slot is null)
    {
      throw new InvalidOperationException($"The Pokémon 'Id={Id}' is not owned by any trainer.");
    }
    else if (pokemon.Ownership is null || pokemon.Slot is null)
    {
      throw new ArgumentException("The Pokémon is not owned by any trainer.", nameof(pokemon));
    }
    else if (Ownership.TrainerId != pokemon.Ownership.TrainerId)
    {
      throw new ArgumentException("The Pokémon are not owned by the same trainer.", nameof(pokemon));
    }
    else if (Slot != pokemon.Slot)
    {
      PokemonSlot slot = Slot;
      Raise(new PokemonSwapped(pokemon.Slot), actorId);
      pokemon.Raise(new PokemonSwapped(slot), actorId);
    }
  }
  protected virtual void Handle(PokemonSwapped @event)
  {
    Slot = @event.Slot;
  }

  public void SwitchMoves(int source, int destination, ActorId? actorId = null)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(source, nameof(source));
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(source, MoveLimit, nameof(source));

    ArgumentOutOfRangeException.ThrowIfNegative(destination, nameof(destination));
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(destination, MoveLimit, nameof(destination));

    if (source != destination && source < _currentMoves.Count && destination < _currentMoves.Count)
    {
      Raise(new PokemonMoveSwitched(source, destination), actorId);
    }
  }
  protected virtual void Handle(PokemonMoveSwitched @event)
  {
    MoveId source = _currentMoves[@event.Source];
    MoveId destination = _currentMoves[@event.Destination];

    _currentMoves[@event.Source] = destination;
    _currentMoves[@event.Destination] = source;
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
    if (@event.IsShiny.HasValue)
    {
      _isShiny = @event.IsShiny.Value;
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
    if (@event.Friendship is not null)
    {
      _friendship = @event.Friendship;
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

  public void Withdraw(Position position, ActorId? actorId = null)
  {
    if (Ownership is null)
    {
      throw new InvalidOperationException($"The Pokémon 'Id={Id}' is not owned by any trainer.");
    }

    PokemonSlot slot = new(position, Box: null);
    if (Slot != slot)
    {
      Raise(new PokemonWithdrew(slot), actorId);
    }
  }
  protected virtual void Handle(PokemonWithdrew @event)
  {
    Slot = @event.Slot;
  }

  private void SetOwnership(OwnershipKind kind, Trainer trainer, Item pokeBall, Location location, Level? level, DateTime? metOn, Description? description, PokemonSlot? slot, ActorId? actorId)
  {
    if (pokeBall.Category != ItemCategory.PokeBall)
    {
      throw new ArgumentException($"The item category should be '{ItemCategory.PokeBall}'.", nameof(pokeBall));
    }
    if (level is not null)
    {
      ArgumentOutOfRangeException.ThrowIfGreaterThan(level.Value, Level, nameof(level));
    }
    if (metOn?.AsUniversalTime() > DateTime.UtcNow)
    {
      throw new ArgumentOutOfRangeException(nameof(metOn));
    }

    ItemId pokeBallId = Ownership is null ? pokeBall.Id : Ownership.PokeBallId;
    level ??= new(Level);
    slot ??= new(new Position(0), Box: null);

    switch (kind)
    {
      case OwnershipKind.Caught:
        Raise(new PokemonCaught(trainer.Id, pokeBallId, level, location, metOn, description, slot), actorId);
        break;
      case OwnershipKind.Received:
        Raise(new PokemonReceived(trainer.Id, pokeBallId, level, location, metOn, description, slot), actorId);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(kind));
    }
  }
  protected virtual void Handle(PokemonCaught @event)
  {
    OriginalTrainerId = @event.TrainerId;
    Ownership = new Ownership(OwnershipKind.Caught, @event.TrainerId, @event.PokeBallId, @event.Level, @event.Location, @event.MetOn ?? @event.OccurredOn, @event.Description);
    Slot = @event.Slot;
  }
  protected virtual void Handle(PokemonReceived @event)
  {
    OriginalTrainerId ??= @event.TrainerId;
    Ownership = new Ownership(OwnershipKind.Received, @event.TrainerId, @event.PokeBallId, @event.Level, @event.Location, @event.MetOn ?? @event.OccurredOn, @event.Description);
    Slot = @event.Slot;
  }

  public override string ToString() => $"{Nickname?.Value ?? UniqueName.Value} | {base.ToString()}";
}
