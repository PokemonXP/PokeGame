using FluentValidation;

namespace PokeGame.Core.Forms;

public record Height
{
  public int Value { get; }

  public Height(int value)
  {
    Value = value;
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Height>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Height();
    }
  }
}
