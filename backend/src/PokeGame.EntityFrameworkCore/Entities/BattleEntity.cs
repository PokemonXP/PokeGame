using Logitar;
using Logitar.EventSourcing;
using PokeGame.Core.Battles;
using PokeGame.Core.Battles.Events;
using PokeGame.Core.Pokemon;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class BattleEntity : AggregateEntity
{
  public int BattleId { get; private set; }
  public Guid Id { get; private set; }

  public BattleKind Kind { get; private set; }
  public BattleStatus Status { get; private set; }
  public BattleResolution? Resolution { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string Location { get; private set; } = string.Empty;
  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public int ChampionCount { get; private set; }
  public int OpponentCount { get; private set; }

  public string? StartedBy { get; private set; }
  public DateTime? StartedOn { get; private set; }
  public string? CancelledBy { get; private set; }
  public DateTime? CancelledOn { get; private set; }
  public string? CompletedBy { get; private set; }
  public DateTime? CompletedOn { get; private set; }

  public List<BattlePokemonEntity> Pokemon { get; private set; } = [];
  public List<BattleTrainerEntity> Trainers { get; private set; } = [];

  public BattleEntity(IEnumerable<TrainerEntity> champions, IEnumerable<TrainerEntity> opponents, TrainerBattleCreated @event) : this(@event)
  {
    Id = new BattleId(@event.StreamId).ToGuid();

    Kind = BattleKind.Trainer;

    AddChampions(champions);
    foreach (TrainerEntity opponent in opponents)
    {
      Trainers.Add(new BattleTrainerEntity(this, opponent, isOpponent: true));
    }
    OpponentCount = opponents.Count();
  }
  public BattleEntity(IEnumerable<TrainerEntity> champions, IEnumerable<PokemonEntity> opponents, WildPokemonBattleCreated @event) : this(@event)
  {
    Id = new BattleId(@event.StreamId).ToGuid();

    Kind = BattleKind.WildPokemon;

    AddChampions(champions);
    foreach (PokemonEntity opponent in opponents)
    {
      Pokemon.Add(new BattlePokemonEntity(this, opponent, isActive: true));
    }
    OpponentCount = opponents.Count();
  }
  private BattleEntity(IBattleCreated @event) : base((DomainEvent)@event)
  {
    Status = BattleStatus.Created;

    Name = @event.Name.Value;
    Location = @event.Location.Value;
    Url = @event.Url?.Value;
    Notes = @event.Notes?.Value;
  }

  private BattleEntity() : base()
  {
  }

  public void Cancel(BattleCancelled @event)
  {
    Update(@event);

    Status = BattleStatus.Cancelled;

    CancelledBy = @event.ActorId?.Value;
    CancelledOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Escape(BattleEscaped @event)
  {
    Update(@event);

    Status = BattleStatus.Completed;
    Resolution = BattleResolution.Escape;

    CompletedBy = @event.ActorId?.Value;
    CompletedOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Reset(BattleReset @event)
  {
    Update(@event);

    Status = BattleStatus.Created;

    StartedBy = null;
    StartedOn = null;
  }

  public void Start(IEnumerable<PokemonEntity> pokemon, BattleStarted @event)
  {
    Update(@event);

    Status = BattleStatus.Started;

    StartedBy = @event.ActorId?.Value;
    StartedOn = @event.OccurredOn.AsUniversalTime();

    foreach (PokemonEntity specimen in pokemon)
    {
      bool isActive = @event.PokemonIds[new PokemonId(specimen.Id)];
      Pokemon.Add(new BattlePokemonEntity(this, specimen, isActive));
    }
  }

  public void SwitchPokemon(BattlePokemonSwitched @event)
  {
    Update(@event);

    BattlePokemonEntity? active = Pokemon.SingleOrDefault(x => x.Pokemon?.StreamId == @event.ActiveId.Value);
    BattlePokemonEntity? inactive = Pokemon.SingleOrDefault(x => x.Pokemon?.StreamId == @event.InactiveId.Value);
    if (active is not null && inactive is not null)
    {
      active.Switch(inactive);
    }
  }

  public void Update(BattleUpdated @event)
  {
    base.Update(@event);

    if (@event.Name is not null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Location is not null)
    {
      Location = @event.Location.Value;
    }
    if (@event.Url is not null)
    {
      Url = @event.Url.Value?.Value;
    }
    if (@event.Notes is not null)
    {
      Notes = @event.Notes.Value?.Value;
    }
  }

  private void AddChampions(IEnumerable<TrainerEntity> champions)
  {
    foreach (TrainerEntity champion in champions)
    {
      Trainers.Add(new BattleTrainerEntity(this, champion, isOpponent: false));
    }
    ChampionCount = champions.Count();
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
