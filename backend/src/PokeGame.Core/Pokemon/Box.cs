using FluentValidation;

namespace PokeGame.Core.Pokemon;

public record Box
{
  public int Value { get; }

  public Box(int value)
  {
    Value = value;
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Box>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Box();
    }
  }
}
