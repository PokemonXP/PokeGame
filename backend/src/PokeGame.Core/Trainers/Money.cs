using FluentValidation;

namespace PokeGame.Core.Trainers;

public record Money
{
  public int Value { get; }

  public Money() : this(value: 0)
  {
  }

  public Money(int value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Money>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Money();
    }
  }
}
