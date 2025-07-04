using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class StatusHealingValidator : AbstractValidator<StatusHealingPayload>
{
  public StatusHealingValidator()
  {
    RuleFor(x => x.Condition).IsInEnum();
    When(x => x.Condition.HasValue, () => RuleFor(x => x.All).Equal(false));
    When(x => x.All, () => RuleFor(x => x.Condition).Null());
  }
}
