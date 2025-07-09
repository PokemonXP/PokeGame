using FluentValidation;

namespace PokeGame.Core;

public record GameLocation
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public GameLocation(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<GameLocation>
  {
    public Validator()
    {
      RuleFor(x => x.Value).NotEmpty().MaximumLength(MaximumLength);
    }
  }
}
