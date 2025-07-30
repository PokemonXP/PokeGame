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

  private readonly HashSet<TrainerId> _trainerOpponents = [];
  public IReadOnlyCollection<TrainerId> TrainerOpponents => _trainerOpponents.ToList().AsReadOnly();

  private readonly HashSet<PokemonId> _pokemonOpponents = [];
  public IReadOnlyCollection<PokemonId> PokemonOpponents => _pokemonOpponents.ToList().AsReadOnly();

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
      throw new ArgumentException("At least one opponent trainer must be provided.", nameof(champions));
    }

    IEnumerable<Trainer> intersection = champions.Intersect(opponents);
    if (intersection.Any())
    {
      StringBuilder message = new();
      message.AppendLine("The following trainers have been provided in champions and opponents. A trainer may only appear on one side.");
      foreach (Trainer trainer in intersection)
      {
        message.Append(" - ").Append(trainer).AppendLine();
      }
      throw new InvalidOperationException(message.ToString());
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
    _trainerOpponents.AddRange(@event.OpponentIds);
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
      throw new ArgumentException("At least one opponent Pokémon must be provided.", nameof(champions));
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
      StringBuilder message = new();
      message.AppendLine("The following Pokémon are not wild Pokémon.");
      foreach (Specimen pokemon in trainerPokemon)
      {
        message.Append(" - ").Append(pokemon).AppendLine();
      }
      throw new ArgumentException(message.ToString(), nameof(opponents));
    }
    if (eggPokemon.Count > 0)
    {
      StringBuilder message = new();
      message.AppendLine("The following Pokémon are still egg Pokémon.");
      foreach (Specimen pokemon in trainerPokemon)
      {
        message.Append(" - ").Append(pokemon).AppendLine();
      }
      throw new ArgumentException(message.ToString(), nameof(opponents));
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
    _pokemonOpponents.AddRange(@event.OpponentIds);
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
}
