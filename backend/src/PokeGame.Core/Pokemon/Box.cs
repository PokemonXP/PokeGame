using FluentValidation;

namespace PokeGame.Core.Pokemon;

public record Box
{
  public const int Count = 32;
  public const int Size = 5 * 6;

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
      RuleFor(x => x.Value).InclusiveBetween(0, Count - 1);
    }
  }
}
