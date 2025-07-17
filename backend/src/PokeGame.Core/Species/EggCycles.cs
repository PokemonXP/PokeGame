using FluentValidation;

namespace PokeGame.Core.Species;

public record EggCycles
{
  public byte Value { get; }

  public EggCycles(byte value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<EggCycles>
  {
    public Validator()
    {
      RuleFor(x => x.Value).GreaterThan((byte)0);
    }
  }
}
