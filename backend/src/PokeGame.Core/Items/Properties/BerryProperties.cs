using FluentValidation;
using PokeGame.Core.Items.Validators;

namespace PokeGame.Core.Items.Properties;

public record BerryProperties : ItemProperties, IBerryProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Berry;

  public int Healing { get; }
  public bool IsHealingPercentage { get; }

  public StatusCondition? StatusCondition { get; }
  public bool AllConditions { get; }
  public bool CureConfusion { get; }

  public int PowerPoints { get; }

  public int Attack { get; }
  public int Defense { get; }
  public int SpecialAttack { get; }
  public int SpecialDefense { get; }
  public int Speed { get; }
  public int Accuracy { get; }
  public int Evasion { get; }
  public int Critical { get; }

  public PokemonStatistic? LowerEffortValues { get; }
  public bool RaiseFriendship { get; }

  public BerryProperties()
  {
  }

  [JsonConstructor]
  public BerryProperties(
    int healing,
    bool isHealingPercentage,
    StatusCondition? statusCondition,
    bool allConditions,
    bool cureConfusion,
    int powerPoints,
    int attack,
    int defense,
    int specialAttack,
    int specialDefense,
    int speed,
    int accuracy,
    int evasion,
    int critical,
    PokemonStatistic? lowerEffortValues,
    bool raiseFriendship)
  {
    Healing = healing;
    IsHealingPercentage = isHealingPercentage;

    StatusCondition = statusCondition;
    AllConditions = allConditions;
    CureConfusion = cureConfusion;

    PowerPoints = powerPoints;

    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
    Accuracy = accuracy;
    Evasion = evasion;
    Critical = critical;

    LowerEffortValues = lowerEffortValues;
    RaiseFriendship = raiseFriendship;

    new BerryValidator().ValidateAndThrow(this);
  }

  public BerryProperties(IBerryProperties berry) : this(
    berry.Healing,
    berry.IsHealingPercentage,
    berry.StatusCondition,
    berry.AllConditions,
    berry.CureConfusion,
    berry.PowerPoints,
    berry.Attack,
    berry.Defense,
    berry.SpecialAttack,
    berry.SpecialDefense,
    berry.Speed,
    berry.Accuracy,
    berry.Evasion,
    berry.Critical,
    berry.LowerEffortValues,
    berry.RaiseFriendship)
  {
  }
}
