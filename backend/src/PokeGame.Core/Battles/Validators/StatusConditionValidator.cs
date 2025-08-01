using FluentValidation;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Validators;

internal class StatusConditionValidator : AbstractValidator<StatusConditionPayload>
{
  public StatusConditionValidator()
  {
    RuleFor(x => x.Condition).IsInEnum();
    When(x => x.AllConditions, () =>
    {
      RuleFor(x => x.Condition).Null();
      RuleFor(x => x.RemoveCondition).Equal(true);
    }).Otherwise(() => RuleFor(x => x.Condition).NotNull());
  }
}
