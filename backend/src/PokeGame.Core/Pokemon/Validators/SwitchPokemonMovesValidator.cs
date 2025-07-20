using FluentValidation;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Validators;

internal class SwitchPokemonMovesValidator : AbstractValidator<SwitchPokemonMovesPayload>
{
  public SwitchPokemonMovesValidator()
  {
    RuleFor(x => x.Source).InclusiveBetween(0, Specimen.MoveLimit - 1);
    RuleFor(x => x.Destination).InclusiveBetween(0, Specimen.MoveLimit - 1);
  }
}
