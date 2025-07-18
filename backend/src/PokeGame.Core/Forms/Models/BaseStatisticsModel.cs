namespace PokeGame.Core.Forms.Models;

public record BaseStatisticsModel : IBaseStatistics
{
  public byte HP { get; set; }
  public byte Attack { get; set; }
  public byte Defense { get; set; }
  public byte SpecialAttack { get; set; }
  public byte SpecialDefense { get; set; }
  public byte Speed { get; set; }

  public BaseStatisticsModel()
  {
  }

  public BaseStatisticsModel(byte hp, byte attack, byte defense, byte specialAttack, byte specialDefense, byte speed)
  {
    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
  }

  public BaseStatisticsModel(IBaseStatistics @base) : this(@base.HP, @base.Attack, @base.Defense, @base.SpecialAttack, @base.SpecialDefense, @base.Speed)
  {

  }
}
