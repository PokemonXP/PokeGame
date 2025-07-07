using FluentValidation;

namespace PokeGame.Core.Pokemons;

public record IndividualValues
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

  public IndividualValues(byte hp, byte attack, byte defense, byte specialAttack, byte specialDefense, byte speed)
  {
    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<IndividualValues>
  {
    public Validator()
    {
      RuleFor(x => x.HP).InclusiveBetween((byte)0, (byte)31);
      RuleFor(x => x.Attack).InclusiveBetween((byte)0, (byte)31);
      RuleFor(x => x.Defense).InclusiveBetween((byte)0, (byte)31);
      RuleFor(x => x.SpecialAttack).InclusiveBetween((byte)0, (byte)31);
      RuleFor(x => x.SpecialDefense).InclusiveBetween((byte)0, (byte)31);
      RuleFor(x => x.Speed).InclusiveBetween((byte)0, (byte)31);
    }
  }
}
