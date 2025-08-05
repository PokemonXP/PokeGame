using FluentValidation;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon.Validators;

namespace PokeGame.Core.Items.Validators;

internal class BerryValidator : AbstractValidator<IBerryProperties>
{
  public BerryValidator()
  {
    When(x => x.IsHealingPercentage, () => RuleFor(x => x.Healing).InclusiveBetween(1, 100))
      .Otherwise(() => RuleFor(x => x.Healing).GreaterThanOrEqualTo(0));

    RuleFor(x => x.StatusCondition).IsInEnum();
    When(x => x.AllConditions, () => RuleFor(x => x.StatusCondition).Null());
    When(x => x.CureConfusion, () => RuleFor(x => x.StatusCondition).Null());

    RuleFor(x => x.PowerPoints).InclusiveBetween(0, PowerPoints.MaximumValue);

    RuleFor(x => x).SetValidator(new StatisticChangesValidator());

    RuleFor(x => x.LowerEffortValues).IsInEnum();
  }
}
