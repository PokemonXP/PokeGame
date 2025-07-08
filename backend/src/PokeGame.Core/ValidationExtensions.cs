using FluentValidation;
using PokeGame.Core.Validators;

namespace PokeGame.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> PokemonNature<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.SetValidator(new PokemonNatureValidator<T>());
  }
}
