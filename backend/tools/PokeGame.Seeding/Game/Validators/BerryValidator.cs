using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class BerryValidator : AbstractValidator<BerryPayload>
{
  public BerryValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Price).GreaterThanOrEqualTo(0);

    RuleFor(x => x.Healing).GreaterThanOrEqualTo(0);
    When(x => x.IsHealingPercentage, () => RuleFor(x => x.Healing).InclusiveBetween(1, 100));

    RuleFor(x => x.StatusCondition).IsInEnum();

    RuleFor(x => x.PowerPoints).InclusiveBetween(0, 40);

    RuleFor(x => x.StatisticChanges).SetValidator(new StatisticChangesValidator());
    RuleFor(x => x.CriticalChange).InclusiveBetween(0, 4);

    RuleFor(x => x.LowerEffortValues).IsInEnum();

    When(x => !string.IsNullOrWhiteSpace(x.Sprite), () => RuleFor(x => x.Sprite!).Url());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
  }
}
