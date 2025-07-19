using FluentValidation;

namespace PokeGame.Core.Regions;

public record Location
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Location(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Location>
  {
    public Validator()
    {
      RuleFor(x => x.Value).NotEmpty().MaximumLength(MaximumLength);
    }
  }
}
