namespace PokeGame.Core.Forms.Models;

public record YieldModel : IYield
{
  public int Experience { get; set; }

  public int HP { get; set; }
  public int Attack { get; set; }
  public int Defense { get; set; }
  public int SpecialAttack { get; set; }
  public int SpecialDefense { get; set; }
  public int Speed { get; set; }

  public YieldModel()
  {
  }

  public YieldModel(int experience, int hp, int attack, int defense, int specialAttack, int specialDefense, int speed)
  {
    Experience = experience;

    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
  }

  public YieldModel(IYield yield) : this(yield.Experience, yield.HP, yield.Attack, yield.Defense, yield.SpecialAttack, yield.SpecialDefense, yield.Speed)
  {
  }
}
