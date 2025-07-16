using FluentValidation;
using PokeGame.Core.Pokemons;

namespace PokeGame.Core.Varieties;

public record GenderRatio
{
  public const int MinimumValue = 0;
  public const int MaximumValue = 8;

  public int Value { get; set; }

  public GenderRatio(int value)
  {
    Value = value;
  }

  public bool IsValid(PokemonGender gender) => Value switch
  {
    MinimumValue => gender == PokemonGender.Female,
    MaximumValue => gender == PokemonGender.Male,
    _ => true,
  };

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<GenderRatio>
  {
    public Validator()
    {
      RuleFor(x => x.Value).GenderRatio();
    }
  }
}
