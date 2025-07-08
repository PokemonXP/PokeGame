namespace PokeGame.Core.Pokemons.Models;

public record StatisticsModel
{
  public StatisticModel HP { get; set; } = new();
  public StatisticModel Attack { get; set; } = new();
  public StatisticModel Defense { get; set; } = new();
  public StatisticModel SpecialAttack { get; set; } = new();
  public StatisticModel SpecialDefense { get; set; } = new();
  public StatisticModel Speed { get; set; } = new();

  public StatisticsModel()
  {
  }

  public StatisticsModel(StatisticModel hp, StatisticModel attack, StatisticModel defense, StatisticModel specialAttack, StatisticModel specialDefense, StatisticModel speed)
  {
    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
  }
}

public record StatisticModel
{
  public byte Base { get; set; }
  public byte IndividualValue { get; set; }
  public byte EffortValue { get; set; }
  public int Value { get; set; }

  public StatisticModel()
  {
  }

  public StatisticModel(byte @base, byte individualValue, byte effortValue, int value)
  {
    Base = @base;
    IndividualValue = individualValue;
    EffortValue = effortValue;
    Value = value;
  }
}
