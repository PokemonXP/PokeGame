using FluentValidation;
using PokeGame.Core.Pokemons.Validators;

namespace PokeGame.Core.Pokemons;

public record BaseStatistics : IBaseStatistics
{
  public byte HP { get; }
  public byte Attack { get; }
  public byte Defense { get; }
  public byte SpecialAttack { get; }
  public byte SpecialDefense { get; }
  public byte Speed { get; }

  public BaseStatistics()
  {
  }

  public BaseStatistics(byte hp, byte attack, byte defense, byte specialAttack, byte specialDefense, byte speed)
  {
    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
    new BaseStatisticsValidator().ValidateAndThrow(this);
  }

  public BaseStatistics(IBaseStatistics @base) : this(@base.HP, @base.Attack, @base.Defense, @base.SpecialAttack, @base.SpecialDefense, @base.Speed)
  {
  }
}
