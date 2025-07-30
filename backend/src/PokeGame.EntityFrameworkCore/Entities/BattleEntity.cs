using Logitar.EventSourcing;
using PokeGame.Core.Battles;
using PokeGame.Core.Battles.Events;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class BattleEntity : AggregateEntity
{
  public int BattleId { get; private set; }
  public Guid Id { get; private set; }

  public BattleKind Kind { get; private set; }
  public BattleStatus Status { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string Location { get; private set; } = string.Empty;
  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<BattlePokemonEntity> Pokemon { get; private set; } = [];
  public List<BattleTrainerEntity> Trainers { get; private set; } = [];

  public BattleEntity(List<TrainerEntity> champions, List<TrainerEntity> opponents, TrainerBattleCreated @event) : this(champions, @event)
  {
    Id = new BattleId(@event.StreamId).ToGuid();

    Kind = BattleKind.Trainer;

    foreach (TrainerEntity opponent in opponents)
    {
      Trainers.Add(new BattleTrainerEntity(this, opponent, isOpponent: true));
    }
  }
  public BattleEntity(List<TrainerEntity> champions, List<PokemonEntity> opponents, WildPokemonBattleCreated @event) : this(champions, @event)
  {
    Id = new BattleId(@event.StreamId).ToGuid();

    Kind = BattleKind.WildPokemon;

    foreach (PokemonEntity opponent in opponents)
    {
      Pokemon.Add(new BattlePokemonEntity(this, opponent));
    }
  }
  private BattleEntity(List<TrainerEntity> champions, IBattleCreated @event) : base((DomainEvent)@event)
  {
    Status = BattleStatus.Created;

    Name = @event.Name.Value;
    Location = @event.Location.Value;
    Url = @event.Url?.Value;
    Notes = @event.Notes?.Value;

    foreach (TrainerEntity champion in champions)
    {
      Trainers.Add(new BattleTrainerEntity(this, champion, isOpponent: false));
    }
  }

  private BattleEntity() : base()
  {
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
