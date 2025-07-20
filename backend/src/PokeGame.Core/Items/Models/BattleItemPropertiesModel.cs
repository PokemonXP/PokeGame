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

  [JsonConstructor]
  public BattleItemPropertiesModel(
    int attack = 0,
    int defense = 0,
    int specialAttack = 0,
    int specialDefense = 0,
    int speed = 0,
    int accuracy = 0,
    int evasion = 0,
    int critical = 0,
    int guardTurns = 0)
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
