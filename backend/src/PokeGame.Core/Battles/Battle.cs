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
  public new BattleId Id => new(base.Id);

  public BattleKind Kind { get; private set; }
  public BattleStatus Status { get; private set; }

  private DisplayName? _name = null;
  public DisplayName Name => _name ?? throw new InvalidOperationException("The battle has not been initialized.");
  private Location? _location = null;
  public Location Location => _location ?? throw new InvalidOperationException("The battle has not been initialized.");
  public Url? Url { get; private set; }
  public Notes? Notes { get; private set; }

  private readonly HashSet<TrainerId> _champions = [];
  public IReadOnlyCollection<TrainerId> Champions => _champions.ToList().AsReadOnly();

  private readonly HashSet<TrainerId> _opponents = [];
  public IReadOnlyCollection<TrainerId> Opponents => _opponents.ToList().AsReadOnly();

  private readonly HashSet<PokemonId> _pokemon = [];
  public IReadOnlyCollection<PokemonId> Pokemon => _pokemon.ToList().AsReadOnly();

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
    _pokemon.AddRange(@event.OpponentIds);
  }

  protected virtual void Handle(IBattleCreated @event)
  {
    Status = BattleStatus.Created;

    _name = @event.Name;
    _location = @event.Location;
    Url = @event.Url;
    Notes = @event.Notes;

    _champions.Clear();
    _champions.AddRange(@event.ChampionIds);
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new BattleDeleted(), actorId);
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

    Raise(new BattleStarted(pokemon.Select(x => x.Id).ToHashSet()), actorId); // TODO(fpion): currently active Pokémon
  }
  protected virtual void Handle(BattleStarted @event)
  {
    Status = BattleStatus.Started;

    _pokemon.AddRange(@event.PokemonIds);
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
