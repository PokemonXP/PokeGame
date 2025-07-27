using FluentValidation;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Validators;

internal class MovePokemonValidator : AbstractValidator<MovePokemonPayload>
{
  public MovePokemonValidator()
  {
    //RuleFor(x => x.Position).InclusiveBetween(0, Storage.BoxSize - 1);
    //RuleFor(x => x.Box).Box(); // TODO(fpion): refactor
  }
}
