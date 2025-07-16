using FluentValidation;

namespace PokeGame.Core;

public record Notes
{
  public string Value { get; }

  public Notes(string value)
  {
    Value = value.Trim();
  }

  public static Notes? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Notes>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Notes();
    }
  }
}
