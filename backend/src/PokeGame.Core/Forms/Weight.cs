using FluentValidation;

namespace PokeGame.Core.Forms;

public record Weight
{
  public int Value { get; }

  public Weight(int value)
  {
    Value = value;
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Weight>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Weight();
    }
  }
}
