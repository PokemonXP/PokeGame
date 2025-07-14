using FluentValidation;
using PokeGame.Core.Pokemons.Validators;

namespace PokeGame.Core.Pokemons;

public record IndividualValues : IIndividualValues
{
  public byte HP { get; }
  public byte Attack { get; }
  public byte Defense { get; }
  public byte SpecialAttack { get; }
  public byte SpecialDefense { get; }
  public byte Speed { get; }

  public IndividualValues()
  {
  }

  [JsonConstructor]
  public IndividualValues(byte hp, byte attack, byte defense, byte specialAttack, byte specialDefense, byte speed)
  {
    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
    new IndividualValuesValidator().ValidateAndThrow(this);
  }

  public IndividualValues(IIndividualValues iv) : this(iv.HP, iv.Attack, iv.Defense, iv.SpecialAttack, iv.SpecialDefense, iv.Speed)
  {
  }
}
