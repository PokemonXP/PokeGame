using FluentValidation;

namespace PokeGame.Core.Pokemon;

public record Position
{
  public int Value { get; }

  public Position(int value)
  {
    Value = value;
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Position>
  {
    public Validator()
    {
      RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
    }
  }
}
