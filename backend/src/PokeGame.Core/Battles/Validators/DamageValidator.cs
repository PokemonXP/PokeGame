using FluentValidation;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Validators;

internal class DamageValidator : AbstractValidator<DamagePayload>
{
  public DamageValidator()
  {
    RuleFor(x => x.Value).GreaterThan(0);
    When(x => x.IsPercentage, () => RuleFor(x => x.Value).LessThanOrEqualTo(100));
  }
}
