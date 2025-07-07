using FluentValidation;
using PokeGame.Core.Pokemons.Validators;

namespace PokeGame.Core.Pokemons;

public record EffortValues : IEffortValues
{
  public byte HP { get; }
  public byte Attack { get; }
  public byte Defense { get; }
  public byte SpecialAttack { get; }
  public byte SpecialDefense { get; }
  public byte Speed { get; }

  public EffortValues()
  {
  }

  public EffortValues(byte hp, byte attack, byte defense, byte specialAttack, byte specialDefense, byte speed)
  {
    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
    new EffortValuesValidator().ValidateAndThrow(this);
  }

  public EffortValues(IEffortValues ev) : this(ev.HP, ev.Attack, ev.Defense, ev.SpecialAttack, ev.SpecialDefense, ev.Speed)
  {
  }
}
