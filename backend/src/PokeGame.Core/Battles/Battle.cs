using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core;
using Logitar;
using Logitar.EventSourcing;
using PokeGame.Core.Battles.Events;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Battles;

public class Battle : AggregateRoot
{
  private BattleUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Location is not null || _updated.Url is not null || _updated.Notes is not null;

  public new BattleId Id => new(base.Id);

  public BattleKind Kind { get; private set; }
  public BattleStatus Status { get; private set; }

  private DisplayName? _name = null;
  public DisplayName Name
  {
    get => _name ?? throw new InvalidOperationException("The battle has not been initialized.");
    set
    {
      if (_name != value)
      {
        _name = value;
        _updated.Name = value;
      }
    }
  }
  private Location? _location = null;
  public Location Location
  {
    get => _location ?? throw new InvalidOperationException("The battle has not been initialized.");
    set
    {
      if (_location != value)
      {
        _location = value;
        _updated.Location = value;
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
        _updated.Notes = new Change<Notes>(value);
      }
    }
  }

  private readonly HashSet<TrainerId> _champions = [];
  public IReadOnlyCollection<TrainerId> Champions => _champions.ToList().AsReadOnly();

  private readonly HashSet<TrainerId> _opponents = [];
  public IReadOnlyCollection<TrainerId> Opponents => _opponents.ToList().AsReadOnly();

  private readonly Dictionary<PokemonId, bool> _pokemon = [];
  public IReadOnlyDictionary<PokemonId, bool> Pokemon => _pokemon.AsReadOnly();

  public Battle() : base()
  {
  }

  private Battle(BattleId? id = null) : base((id ?? BattleId.NewId()).StreamId)
  {
  }

  public static Battle Trainer(
    DisplayName name,
    Location location,
    IReadOnlyCollection<Trainer> champions,
    IReadOnlyCollection<Trainer> opponents,
    Url? url = null,
    Notes? notes = null,
    ActorId? actorId = null,
    BattleId? battleId = null)
  {
    if (champions.Count < 1)
    {
      throw new ArgumentException("At least one champion trainer must be provided.", nameof(champions));
    }
    if (opponents.Count < 1)
    {
      throw new ArgumentException("At least one opponent trainer must be provided.", nameof(opponents));
    }

    IEnumerable<Trainer> intersection = champions.Intersect(opponents);
    if (intersection.Any())
    {
      throw new ValidationException(intersection.Select(trainer => new ValidationFailure("TrainerId", "The trainer cannot appear on both sides of the battle.", trainer.Id.ToGuid())
      {
        ErrorCode = "TrainerBattleValidator"
      }));
    }

    Battle battle = new(battleId);

    IReadOnlyCollection<TrainerId> championIds = champions.Select(x => x.Id).ToHashSet();
    IReadOnlyCollection<TrainerId> opponentIds = opponents.Select(x => x.Id).ToHashSet();
    battle.Raise(new TrainerBattleCreated(championIds, opponentIds, name, location, url, notes), actorId);

    return battle;
  }
  protected virtual void Handle(TrainerBattleCreated @event)
  {
    Kind = BattleKind.Trainer;

    Handle((IBattleCreated)@event);
    _opponents.AddRange(@event.OpponentIds);
  }

  public static Battle WildPokemon(
    DisplayName name,
    Location location,
    IReadOnlyCollection<Trainer> champions,
    IReadOnlyCollection<Specimen> opponents,
    Url? url = null,
    Notes? notes = null,
    ActorId? actorId = null,
    BattleId? battleId = null)
  {
    if (champions.Count < 1)
    {
      throw new ArgumentException("At least one champion trainer must be provided.", nameof(champions));
    }
    if (opponents.Count < 1)
    {
      throw new ArgumentException("At least one opponent Pokémon must be provided.", nameof(opponents));
    }

    List<Specimen> trainerPokemon = new(capacity: opponents.Count);
    List<Specimen> eggPokemon = new(capacity: opponents.Count);
    foreach (Specimen opponent in opponents)
    {
      if (opponent.OriginalTrainerId.HasValue || opponent.Ownership is not null)
      {
        trainerPokemon.Add(opponent);
      }
      if (opponent.IsEgg)
      {
        eggPokemon.Add(opponent);
      }
    }
    if (trainerPokemon.Count > 0)
    {
      throw new ValidationException(trainerPokemon.Select(pokemon => new ValidationFailure("Opponents", "The Pokémon must be a wild Pokémon.", pokemon.Id.ToGuid())
      {
        ErrorCode = "WildPokemonValidator"
      }));
    }
    if (eggPokemon.Count > 0)
    {
      throw new ValidationException(eggPokemon.Select(pokemon => new ValidationFailure("Opponents", "The Pokémon must not be an egg.", pokemon.Id.ToGuid())
      {
        ErrorCode = "EggValidator"
      }));
    }

    Battle battle = new(battleId);

    IReadOnlyCollection<TrainerId> championIds = champions.Select(x => x.Id).ToHashSet();
    IReadOnlyCollection<PokemonId> opponentIds = opponents.Select(x => x.Id).ToHashSet();
    battle.Raise(new WildPokemonBattleCreated(championIds, opponentIds, name, location, url, notes), actorId);

    return battle;
  }
  protected virtual void Handle(WildPokemonBattleCreated @event)
  {
    Kind = BattleKind.WildPokemon;

    Handle((IBattleCreated)@event);
    foreach (PokemonId opponent in @event.OpponentIds)
    {
      _pokemon[opponent] = true;
    }
  }

  protected virtual void Handle(IBattleCreated @event)
  {
    Status = BattleStatus.Created;

    _name = @event.Name;
    _location = @event.Location;
    _url = @event.Url;
    _notes = @event.Notes;

    _champions.Clear();
    _champions.AddRange(@event.ChampionIds);
  }

  public void Cancel(ActorId? actorId = null)
  {
    if (Status != BattleStatus.Started)
    {
      ValidationFailure failure = new("BattleId", "The battle must have started, but not ended.", Id.ToGuid())
      {
        CustomState = new { Status },
        ErrorCode = "StatusValidator"
      };
      throw new ValidationException([failure]);
    }

    Raise(new BattleCancelled(), actorId);
  }
  protected virtual void Handle(BattleCancelled _)
  {
    Status = BattleStatus.Cancelled;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new BattleDeleted(), actorId);
    }
  }

  public void Reset(IReadOnlyDictionary<PokemonId, Specimen> pokemon, ActorId? actorId = null)
  {
    if (Status != BattleStatus.Started)
    {
      ValidationFailure failure = new("BattleId", "The battle must have started, but not ended.", Id.ToGuid())
      {
        CustomState = new { Status },
        ErrorCode = "StatusValidator"
      };
      throw new ValidationException([failure]);
    }

    HashSet<PokemonId> wildPokemonIds = _pokemon.Where(x => pokemon[x.Key].Ownership is null).Select(x => x.Key).ToHashSet();
    Raise(new BattleReset(wildPokemonIds), actorId);
  }
  protected virtual void Handle(BattleReset @event)
  {
    Status = BattleStatus.Created;

    _pokemon.Clear();
    foreach (PokemonId opponent in @event.OpponentIds)
    {
      _pokemon[opponent] = true;
    }
  }

  public void Start(IEnumerable<Specimen> pokemon, ActorId? actorId = null)
  {
    if (Status != BattleStatus.Created)
    {
      ValidationFailure failure = new("BattleId", "The battle must not have started.", Id.ToGuid())
      {
        CustomState = new { Status },
        ErrorCode = "StatusValidator"
      };
      throw new ValidationException([failure]);
    }

    HashSet<TrainerId> trainerIds = _champions.Concat(_opponents).ToHashSet();
    Dictionary<TrainerId, HashSet<Specimen>> participants = trainerIds.ToDictionary(x => x, x => new HashSet<Specimen>());

    int capacity = pokemon.Count() + trainerIds.Count;
    List<ValidationFailure> failures = new(capacity);
    foreach (Specimen specimen in pokemon)
    {
      if (specimen.IsEgg)
      {
        failures.Add(new ValidationFailure("PokemonId", "The Pokémon must not be an egg.", specimen.Id.ToGuid())
        {
          CustomState = new { EggCycles = specimen.EggCycles?.Value },
          ErrorCode = "NotEggValidator"
        });
      }
      else if (specimen.Ownership is null || !trainerIds.Contains(specimen.Ownership.TrainerId))
      {
        failures.Add(new ValidationFailure("PokemonId", "The Pokémon must be owned by a trainer registered in champions or opponents.", specimen.Id.ToGuid())
        {
          CustomState = new { TrainerId = specimen.Ownership?.TrainerId.ToGuid() },
          ErrorCode = "TrainerValidator"
        });
      }
      else if (specimen.Slot?.Box is not null)
      {
        failures.Add(new ValidationFailure("PokemonId", "The Pokémon must be in the party of its trainer.", specimen.Id.ToGuid())
        {
          CustomState = new { Box = specimen.Slot.Box.Value },
          ErrorCode = "InPartyValidator"
        });
      }
      else
      {
        participants[specimen.Ownership.TrainerId].Add(specimen);
      }
    }
    foreach (KeyValuePair<TrainerId, HashSet<Specimen>> trainer in participants)
    {
      if (trainer.Value.Count < 1)
      {
        failures.Add(new ValidationFailure("TrainerId", "The trainer must have at least one participating Pokémon.", trainer.Key.ToGuid())
        {
          ErrorCode = "NotEmptyPartyValidator"
        });
      }
      else if (trainer.Value.Count > Storage.PartySize)
      {
        failures.Add(new ValidationFailure("TrainerId", $"The trainer party has exceeded the limit ({Storage.PartySize}).", trainer.Key.ToGuid())
        {
          CustomState = new { Party = trainer.Value.Select(x => x.Id.ToGuid()) },
          ErrorCode = "TrainerValidator"
        });
      }
    }
    if (failures.Count > 0)
    {
      throw new ValidationException(failures);
    }

    Dictionary<PokemonId, bool> pokemonIds = pokemon.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => false);
    foreach (KeyValuePair<TrainerId, HashSet<Specimen>> trainer in participants)
    {
      PokemonId activeId = trainer.Value.OrderBy(x => x.Slot?.Position.Value ?? 0).First().Id;
      pokemonIds[activeId] = true;
    }

    Raise(new BattleStarted(pokemonIds.AsReadOnly()), actorId);
  }
  protected virtual void Handle(BattleStarted @event)
  {
    Status = BattleStatus.Started;

    foreach (KeyValuePair<PokemonId, bool> pokemon in @event.PokemonIds)
    {
      _pokemon[pokemon.Key] = pokemon.Value;
    }
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(BattleUpdated @event)
  {
    if (@event.Name is not null)
    {
      _name = @event.Name;
    }
    if (@event.Location is not null)
    {
      _location = @event.Location;
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

  public override string ToString() => $"{Name} | {base.ToString()}";
}
