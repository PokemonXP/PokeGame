using FluentValidation;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Pokemon.Validators;

namespace PokeGame.Core.Battles.Validators;

internal class BattleMoveTargetValidator : AbstractValidator<BattleMoveTargetPayload>
{
  public BattleMoveTargetValidator()
  {
    RuleFor(x => x.Target).NotEmpty();
    When(x => x.Damage is not null, () => RuleFor(x => x.Damage!).SetValidator(new DamageValidator()));
    When(x => x.Status is not null, () => RuleFor(x => x.Status!).SetValidator(new StatusConditionValidator()));
    RuleFor(x => x.Statistics).SetValidator(new StatisticChangesValidator());
  }
}
