using FluentValidation;
using PokeGame.Core.Forms.Validators;

namespace PokeGame.Core.Forms;

public record Yield : IYield
{
  public int Experience { get; }

  public int HP { get; }
  public int Attack { get; }
  public int Defense { get; }
  public int SpecialAttack { get; }
  public int SpecialDefense { get; }
  public int Speed { get; }

  [JsonConstructor]
  public Yield(int experience, int hp, int attack, int defense, int specialAttack, int specialDefense, int speed)
  {
    Experience = experience;

    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;

    new YieldValidator().ValidateAndThrow(this);
  }

  public Yield(IYield yield) : this(yield.Experience, yield.HP, yield.Attack, yield.Defense, yield.SpecialAttack, yield.SpecialDefense, yield.Speed)
  {
  }
}
