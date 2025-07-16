using FluentValidation;
using PokeGame.Core.Validators;

namespace PokeGame.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, int> GenderRatio<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.InclusiveBetween(Varieties.GenderRatio.MinimumValue, Varieties.GenderRatio.MaximumValue);
  }

  public static IRuleBuilderOptions<T, string> Genus<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Varieties.Genus.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Nickname<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Pokemons.Nickname.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Notes<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty();
  }

  public static IRuleBuilderOptions<T, string> PokemonNature<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.SetValidator(new PokemonNatureValidator<T>());
  }
}
