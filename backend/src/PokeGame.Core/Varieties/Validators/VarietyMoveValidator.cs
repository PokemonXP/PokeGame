using FluentValidation;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Varieties.Validators;

internal class VarietyMoveValidator : AbstractValidator<VarietyMovePayload>
{
  public VarietyMoveValidator()
  {
    RuleFor(x => x.Level).InclusiveBetween(0, 100);
  }
}
