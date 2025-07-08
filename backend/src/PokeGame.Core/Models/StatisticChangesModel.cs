namespace PokeGame.Core.Models;

public record StatisticChangesModel
{
  public int Attack { get; set; }
  public int Defense { get; set; }
  public int SpecialAttack { get; set; }
  public int SpecialDefense { get; set; }
  public int Speed { get; set; }
  public int Accuracy { get; set; }
  public int Evasion { get; set; }
  public int Critical { get; set; }

  public StatisticChangesModel()
  {
  }

  public StatisticChangesModel(int attack, int defense, int specialAttack, int specialDefense, int speed, int accuracy, int evasion, int critical)
  {
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
    Accuracy = accuracy;
    Evasion = evasion;
    Critical = critical;
  }
}
