using FluentValidation;

namespace PokeGame.Core.Trainers;

public record License
{
  public const int MaximumLength = 10;

  public string Value { get; }

  public License(string value)
  {
    Value = value.Trim();
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<License>
  {
    public Validator()
    {
      RuleFor(x => x.Value).License();
    }
  }
}
