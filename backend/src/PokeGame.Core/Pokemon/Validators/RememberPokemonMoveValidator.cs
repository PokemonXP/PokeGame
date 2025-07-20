using FluentValidation;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Validators;

internal class RememberPokemonMoveValidator : AbstractValidator<RememberPokemonMovePayload>
{
  public RememberPokemonMoveValidator()
  {
    RuleFor(x => x.Position).InclusiveBetween(0, Specimen.MoveLimit - 1);
  }
}
