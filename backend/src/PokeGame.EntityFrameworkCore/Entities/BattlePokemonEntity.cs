using PokeGame.Core.Battles;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class BattlePokemonEntity
{
  public BattleEntity? Battle { get; private set; }
  public int BattleId { get; private set; }
  public Guid BattleUid { get; private set; }

  public PokemonEntity? Pokemon { get; set; }
  public int PokemonId { get; private set; }
  public Guid PokemonUid { get; private set; }

  public bool IsActive { get; private set; }

  public int Attack { get; private set; }
  public int Defense { get; private set; }
  public int SpecialAttack { get; private set; }
  public int SpecialDefense { get; private set; }
  public int Speed { get; private set; }
  public int Accuracy { get; private set; }
  public int Evasion { get; private set; }
  public int Critical { get; private set; }

  public BattlePokemonEntity(BattleEntity battle, PokemonEntity pokemon, bool isActive)
  {
    Battle = battle;
    BattleId = battle.BattleId;
    BattleUid = battle.Id;

    Pokemon = pokemon;
    PokemonId = pokemon.PokemonId;
    PokemonUid = pokemon.Id;

    IsActive = isActive;
  }

  private BattlePokemonEntity()
  {
  }

  public void Apply(StatisticChanges statisticChanges)
  {
    if (IsActive)
    {
      Attack = CalculateStages(Attack, statisticChanges.Attack);
      Defense = CalculateStages(Defense, statisticChanges.Defense);
      SpecialAttack = CalculateStages(Attack, statisticChanges.SpecialAttack);
      SpecialDefense = CalculateStages(Defense, statisticChanges.SpecialDefense);
      Speed = CalculateStages(Speed, statisticChanges.Speed);
      Accuracy = CalculateStages(Accuracy, statisticChanges.Accuracy);
      Evasion = CalculateStages(Attack, statisticChanges.Evasion);

      int critical = Critical + statisticChanges.Critical;
      Critical = critical < StatisticChanges.MinimumCritical ? StatisticChanges.MinimumCritical
        : critical > StatisticChanges.MaximumCritical ? StatisticChanges.MaximumCritical : critical;
    }
  }
  private static int CalculateStages(int current, int change)
  {
    int result = current + change;
    return result < StatisticChanges.MinimumStage ? StatisticChanges.MinimumStage
      : (result > StatisticChanges.MaximumStage ? StatisticChanges.MaximumStage : current);
  }

  public void Switch(BattlePokemonEntity other)
  {
    (other.IsActive, IsActive) = (IsActive, other.IsActive);

    Attack = 0;
    Defense = 0;
    SpecialAttack = 0;
    SpecialDefense = 0;
    Speed = 0;
    Accuracy = 0;
    Evasion = 0;
    Critical = 0;
  }

  public override bool Equals(object? obj) => obj is BattlePokemonEntity entity && entity.BattleId == BattleId && entity.PokemonId == PokemonId;
  public override int GetHashCode() => HashCode.Combine(BattleId, PokemonId);
  public override string ToString() => $"{GetType()} (BattleId={BattleId}, PokemonId={PokemonId})";
}
