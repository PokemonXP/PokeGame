using FluentValidation;
using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Validators;

internal class MedicineValidator : AbstractValidator<IMedicineProperties>
{
  public MedicineValidator()
  {
    When(x => x.IsHealingPercentage, () => RuleFor(x => x.Healing).InclusiveBetween(1, 100));
    When(x => x.Revives, () => RuleFor(x => x.Healing).GreaterThan(0))
      .Otherwise(() => RuleFor(x => x.Healing).GreaterThanOrEqualTo(0));

    RuleFor(x => x.StatusCondition).IsInEnum();
    When(x => x.AllConditions, () => RuleFor(x => x.StatusCondition).Null());

    When(x => x.IsPowerPointPercentage, () => RuleFor(x => x.PowerPoints).InclusiveBetween(1, 100));
    When(x => x.RestoreAllMoves, () => RuleFor(x => x.PowerPoints).GreaterThan(0))
      .Otherwise(() => RuleFor(x => x.PowerPoints).GreaterThanOrEqualTo(0));
  }
}
