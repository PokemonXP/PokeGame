using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record BerryPropertiesModel : IBerryProperties
{
  public int Healing { get; set; }
  public bool IsHealingPercentage { get; set; }

  public StatusCondition? StatusCondition { get; set; }
  public bool AllConditions { get; set; }
  public bool CureConfusion { get; set; }

  public int PowerPoints { get; set; }

  public int Attack { get; set; }
  public int Defense { get; set; }
  public int SpecialAttack { get; set; }
  public int SpecialDefense { get; set; }
  public int Speed { get; set; }
  public int Accuracy { get; set; }
  public int Evasion { get; set; }
  public int Critical { get; set; }

  public PokemonStatistic? LowerEffortValues { get; set; }
  public bool RaiseFriendship { get; set; }

  public BerryPropertiesModel()
  {
  }

  [JsonConstructor]
  public BerryPropertiesModel(
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
  }

  public BerryPropertiesModel(IBerryProperties berry) : this(
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
