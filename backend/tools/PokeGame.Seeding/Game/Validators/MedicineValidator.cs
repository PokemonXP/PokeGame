using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class MedicineValidator : AbstractValidator<MedicinePayload>
{
  public MedicineValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Price).GreaterThan(0);

    When(x => x.Healing is not null, () => RuleFor(x => x.Healing!).SetValidator(new HealingValidator()));
    When(x => x.Status is not null, () => RuleFor(x => x.Status!).SetValidator(new StatusHealingValidator()));
    When(x => x.PowerPoints is not null, () => RuleFor(x => x.PowerPoints!).SetValidator(new PowerPointRestoreValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Sprite), () => RuleFor(x => x.Sprite!).Url());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
  }
}
