using FluentValidation;

namespace PokeGame.Core.Moves;

public record Power
{
  public byte Value { get; }

  public Power(byte value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Power>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Power();
    }
  }
}
