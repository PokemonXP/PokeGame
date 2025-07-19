using FluentValidation;

namespace PokeGame.Core.Pokemon;

public record Nickname
{
  public const int MaximumLength = 16;

  public string Value { get; }

  public Nickname(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Nickname? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Nickname>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Nickname();
    }
  }
}
