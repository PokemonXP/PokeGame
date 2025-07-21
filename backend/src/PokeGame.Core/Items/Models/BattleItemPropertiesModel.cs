using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record BattleItemPropertiesModel : IBattleItemProperties
{
  public int Attack { get; set; }
  public int Defense { get; set; }
  public int SpecialAttack { get; set; }
  public int SpecialDefense { get; set; }
  public int Speed { get; set; }
  public int Accuracy { get; set; }
  public int Evasion { get; set; }
  public int Critical { get; set; }
  public int GuardTurns { get; set; }

  public BattleItemPropertiesModel()
  {
  }

  [JsonConstructor]
  public BattleItemPropertiesModel(int attack, int defense, int specialAttack, int specialDefense, int speed, int accuracy, int evasion, int critical, int guardTurns)
  {
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
    Accuracy = accuracy;
    Evasion = evasion;
    Critical = critical;
    GuardTurns = guardTurns;
  }

  public BattleItemPropertiesModel(IBattleItemProperties battleItem) : this(
    battleItem.Attack,
    battleItem.Defense,
    battleItem.SpecialAttack,
    battleItem.SpecialDefense,
    battleItem.Speed,
    battleItem.Accuracy,
    battleItem.Evasion,
    battleItem.Critical,
    battleItem.GuardTurns)
  {
  }
}
