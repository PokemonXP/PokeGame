using FluentValidation;

namespace PokeGame.Core.Pokemons;

public record Level
{
  public const int MinimumValue = 1;
  public const int MaximumValue = 100;

  public int Value { get; }

  public Level(int value)
  {
    Value = value;
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Level>
  {
    public Validator()
    {
      RuleFor(x => x.Value).InclusiveBetween(1, 100);
    }
  }
}
