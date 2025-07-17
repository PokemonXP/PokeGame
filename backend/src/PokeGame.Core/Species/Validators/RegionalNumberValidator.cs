using FluentValidation;
using PokeGame.Core.Species.Models;

namespace PokeGame.Core.Species.Validators;

internal class RegionalNumberValidator : AbstractValidator<RegionalNumberPayload>
{
  public RegionalNumberValidator()
  {
    RuleFor(x => x.Number).GreaterThanOrEqualTo(0);
  }
}
