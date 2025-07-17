using FluentValidation;

namespace PokeGame.Core.Species;

public record Number
{
  public int Value { get; }

  public Number(int value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Number>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Number();
    }
  }
}
