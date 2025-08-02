using FluentValidation;
using PokeGame.Core.Pokemon.Validators;

namespace PokeGame.Core.Battles;

public record StatisticChanges : IStatisticChanges
{
  public const int MinimumCritical = 0;
  public const int MaximumCritical = 4;
  public const int MaximumStage = 6;
  public const int MinimumStage = -6;

  public int Attack { get; }
  public int Defense { get; }
  public int SpecialAttack { get; }
  public int SpecialDefense { get; }
  public int Speed { get; }
  public int Accuracy { get; }
  public int Evasion { get; }
  public int Critical { get; }

  public StatisticChanges()
  {
  }

  [JsonConstructor]
  public StatisticChanges(int attack, int defense, int specialAttack, int specialDefense, int speed, int accuracy, int evasion, int critical)
  {
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
    Accuracy = accuracy;
    Evasion = evasion;
    Critical = critical;
    new StatisticChangesValidator().ValidateAndThrow(this);
  }

  public StatisticChanges(IStatisticChanges statistics) : this(
    statistics.Attack,
    statistics.Defense,
    statistics.SpecialAttack,
    statistics.SpecialDefense,
    statistics.Speed,
    statistics.Accuracy,
    statistics.Evasion,
    statistics.Critical)
  {
  }

  public StatisticChanges Apply(IStatisticChanges changes)
  {
    int critical = Critical + changes.Critical;
    return new StatisticChanges(
      CalculateStages(Attack, changes.Attack),
      CalculateStages(Defense, changes.Defense),
      CalculateStages(SpecialAttack, changes.SpecialAttack),
      CalculateStages(SpecialDefense, changes.SpecialDefense),
      CalculateStages(Speed, changes.Speed),
      CalculateStages(Accuracy, changes.Accuracy),
      CalculateStages(Evasion, changes.Evasion),
      critical < MinimumCritical ? MinimumCritical : (critical > MaximumCritical ? MaximumCritical : critical));
  }
  private static int CalculateStages(int current, int change)
  {
    int result = current + change;
    return result < MinimumStage ? MinimumStage : (result > MaximumStage ? MaximumStage : result);
  }
}
