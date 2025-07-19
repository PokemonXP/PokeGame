namespace PokeGame.Core.Pokemon.Models;

public record PokemonStatisticsModel
{
  public PokemonStatisticModel HP { get; set; } = new();
  public PokemonStatisticModel Attack { get; set; } = new();
  public PokemonStatisticModel Defense { get; set; } = new();
  public PokemonStatisticModel SpecialAttack { get; set; } = new();
  public PokemonStatisticModel SpecialDefense { get; set; } = new();
  public PokemonStatisticModel Speed { get; set; } = new();

  public PokemonStatisticsModel()
  {
  }

  public PokemonStatisticsModel(
    PokemonStatisticModel hp,
    PokemonStatisticModel attack,
    PokemonStatisticModel defense,
    PokemonStatisticModel specialAttack,
    PokemonStatisticModel specialDefense,
    PokemonStatisticModel speed)
  {
    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
  }
}

public record PokemonStatisticModel
{
  public byte Base { get; set; }
  public byte IndividualValue { get; set; }
  public byte EffortValue { get; set; }
  public int Value { get; set; }

  public PokemonStatisticModel()
  {
  }

  public PokemonStatisticModel(byte @base, byte individualValue, byte effortValue, int value)
  {
    Base = @base;
    IndividualValue = individualValue;
    EffortValue = effortValue;
    Value = value;
  }
}
