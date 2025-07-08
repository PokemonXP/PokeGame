using FluentValidation;
using FluentValidation.Validators;
using PokeGame.Core.Pokemons;

namespace PokeGame.Core.Validators;

internal class PokemonNatureValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "PokemonNatureValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    string names = string.Join(", ", PokemonNatures.Instance.ToList());
    return $"'{{PropertyName}}' must be one of the following: {names}.";
  }

  public bool IsValid(ValidationContext<T> context, string value) => PokemonNatures.Instance.Get(value) is not null;
}
