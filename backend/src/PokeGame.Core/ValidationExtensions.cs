using FluentValidation;
using PokeGame.Core.Validators;

namespace PokeGame.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, byte> Accuracy<T>(this IRuleBuilder<T, byte> ruleBuilder)
  {
    return ruleBuilder.GreaterThan((byte)0).LessThanOrEqualTo((byte)100);
  }

  public static IRuleBuilderOptions<T, int> GenderRatio<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.InclusiveBetween(Varieties.GenderRatio.MinimumValue, Varieties.GenderRatio.MaximumValue);
  }

  public static IRuleBuilderOptions<T, string> Genus<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Varieties.Genus.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> License<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Trainers.License.MaximumLength);
  }

  public static IRuleBuilderOptions<T, int> Money<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.GreaterThanOrEqualTo(0);
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

  public static IRuleBuilderOptions<T, byte> Power<T>(this IRuleBuilder<T, byte> ruleBuilder)
  {
    return ruleBuilder.GreaterThan((byte)0);
  }

  public static IRuleBuilderOptions<T, int> Price<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.GreaterThan(0);
  }

  public static IRuleBuilderOptions<T, byte> PowerPoints<T>(this IRuleBuilder<T, byte> ruleBuilder)
  {
    return ruleBuilder.GreaterThan((byte)0).LessThanOrEqualTo(Moves.PowerPoints.MaximumValue);
  }
}
