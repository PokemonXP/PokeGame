using FluentValidation;

namespace PokeGame.Core.Speciez;

public record CatchRate
{
  public byte Value { get; }

  public CatchRate(byte value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<CatchRate>
  {
    public Validator()
    {
      RuleFor(x => x.Value).GreaterThan((byte)0);
    }
  }
}
