using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities.Models;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Regions.Models;
using PokeGame.EntityFrameworkCore.Entities;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;
using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.EntityFrameworkCore;

internal class PokemonMapper
{
  private readonly Dictionary<ActorId, Actor> _actors;
  private readonly Actor _system = new();

  public PokemonMapper()
  {
    _actors = [];
  }

  public PokemonMapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors) : this()
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public AbilityModel ToAbility(AbilityEntity source)
  {
    AbilityModel destination = new()
    {
      Id = source.Id,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Url = source.Url,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  public MoveModel ToMove(MoveEntity source)
  {
    MoveModel destination = new()
    {
      Id = source.Id,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Type = source.Type,
      Category = source.Category,
      Accuracy = source.Accuracy,
      Power = source.Power,
      PowerPoints = source.PowerPoints,
      Url = source.Url,
      Notes = source.Notes
    };

    if (source.StatusCondition.HasValue)
    {
      destination.Status = new InflictedStatusModel(source.StatusCondition.Value, source.StatusChance);
    }
    if (source.VolatileConditions is not null)
    {
      destination.VolatileConditions.AddRange(JsonSerializer.Deserialize<string[]>(source.VolatileConditions) ?? []);
    }

    destination.StatisticChanges.Attack = source.AttackChange;
    destination.StatisticChanges.Defense = source.DefenseChange;
    destination.StatisticChanges.SpecialAttack = source.SpecialAttackChange;
    destination.StatisticChanges.SpecialDefense = source.SpecialDefenseChange;
    destination.StatisticChanges.Speed = source.SpeedChange;
    destination.StatisticChanges.Accuracy = source.AccuracyChange;
    destination.StatisticChanges.Evasion = source.EvasionChange;
    destination.StatisticChanges.Critical = source.CriticalChange;

    MapAggregate(source, destination);

    return destination;
  }

  public RegionModel ToRegion(RegionEntity source)
  {
    RegionModel destination = new()
    {
      Id = source.Id,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Url = source.Url,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, AggregateModel destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(string? id) => TryFindActor(id) ?? _system;
  private Actor FindActor(ActorId? id) => TryFindActor(id) ?? _system;
  private Actor? TryFindActor(string? id) => string.IsNullOrWhiteSpace(id) ? null : TryFindActor(new ActorId(id));
  private Actor? TryFindActor(ActorId? id)
  {
    if (id.HasValue)
    {
      return _actors.TryGetValue(id.Value, out Actor? actor) ? actor : null;
    }
    return null;
  }
}
