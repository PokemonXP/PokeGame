using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class BattleItemValidator : AbstractValidator<BattleItemPayload>
{
  public BattleItemValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Price).GreaterThanOrEqualTo(0);

    RuleFor(x => x.StatisticChanges).SetValidator(new StatisticChangesValidator());
    RuleFor(x => x.CriticalChange).InclusiveBetween(0, 4);
    RuleFor(x => x.GuardTurns).InclusiveBetween(0, 10);

    When(x => !string.IsNullOrWhiteSpace(x.Sprite), () => RuleFor(x => x.Sprite!).Url());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
  }
}
