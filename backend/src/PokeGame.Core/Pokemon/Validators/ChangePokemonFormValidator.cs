using FluentValidation;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Validators;

internal class ChangePokemonFormValidator : AbstractValidator<ChangePokemonFormPayload>
{
  public ChangePokemonFormValidator()
  {
    RuleFor(x => x.Form).NotEmpty();
  }
}
