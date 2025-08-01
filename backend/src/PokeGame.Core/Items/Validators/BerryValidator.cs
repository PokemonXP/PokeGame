using FluentValidation;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Moves;

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

    RuleFor(x => x.Attack).InclusiveBetween(-6, 6);
    RuleFor(x => x.Defense).InclusiveBetween(-6, 6);
    RuleFor(x => x.SpecialAttack).InclusiveBetween(-6, 6);
    RuleFor(x => x.SpecialDefense).InclusiveBetween(-6, 6);
    RuleFor(x => x.Speed).InclusiveBetween(-6, 6);
    RuleFor(x => x.Accuracy).InclusiveBetween(-6, 6);
    RuleFor(x => x.Evasion).InclusiveBetween(-6, 6);
    RuleFor(x => x.Critical).InclusiveBetween(0, 4);

    RuleFor(x => x.LowerEffortValues).IsInEnum();
  } // TASK: [POKEGAME-287](https://logitar.atlassian.net/browse/POKEGAME-287)
}
