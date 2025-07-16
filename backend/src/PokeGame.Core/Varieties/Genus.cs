using FluentValidation;

namespace PokeGame.Core.Varieties;

public record Genus
{
  public const int MaximumLength = 16;

  public string Value { get; }

  public Genus(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Genus? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Genus>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Genus();
    }
  }
}
