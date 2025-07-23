using FluentValidation;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Validators;

internal class SwapPokemonValidator : AbstractValidator<SwapPokemonPayload>
{
  public SwapPokemonValidator()
  {
    RuleFor(x => x.Ids).Must(BeValidIds)
      .WithErrorCode("SwapPokemonValidator")
      .WithMessage("'{PropertyName}' must contain exactly 2 distinct elements.");
  }

  private static bool BeValidIds(IEnumerable<Guid> ids) => ids.Distinct().Count() == 2;
}
