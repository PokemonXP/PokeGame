using FluentValidation;
using PokeGame.Core.Items.Validators;

namespace PokeGame.Core.Items.Properties;

public record BattleItemProperties : ItemProperties, IBattleItemProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.BattleItem;

  public int Attack { get; }
  public int Defense { get; }
  public int SpecialAttack { get; }
  public int SpecialDefense { get; }
  public int Speed { get; }
  public int Accuracy { get; }
  public int Evasion { get; }
  public int Critical { get; }
  public int GuardTurns { get; }

  public BattleItemProperties()
  {
  }

  [JsonConstructor]
  public BattleItemProperties(int attack, int defense, int specialAttack, int specialDefense, int speed, int accuracy, int evasion, int critical, int guardTurns)
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
    new BattleItemValidator().ValidateAndThrow(this);
  }

  public BattleItemProperties(IBattleItemProperties battleItem) : this(
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
