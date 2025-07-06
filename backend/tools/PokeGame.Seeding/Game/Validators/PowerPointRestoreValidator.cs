using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class PowerPointRestoreValidator : AbstractValidator<PowerPointRestorePayload>
{
  public PowerPointRestoreValidator()
  {
    RuleFor(x => x.Value).GreaterThan(0);
    When(x => x.IsPercentage, () => RuleFor(x => x.Value).LessThanOrEqualTo(100))
      .Otherwise(() => RuleFor(x => x.Value).LessThanOrEqualTo(40));
  }
}
