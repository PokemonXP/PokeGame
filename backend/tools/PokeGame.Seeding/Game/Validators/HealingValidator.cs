using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class HealingValidator : AbstractValidator<HealingPayload>
{
  public HealingValidator()
  {
    RuleFor(x => x.Value).GreaterThan(0);
    When(x => x.IsPercentage, () => RuleFor(x => x.Value).LessThanOrEqualTo(100));
  }
}
