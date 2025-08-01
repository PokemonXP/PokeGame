using FluentValidation;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Validators;

internal class StatisticChangesValidator : AbstractValidator<StatisticChangesPayload>
{
  public StatisticChangesValidator()
  {
    RuleFor(x => x.Attack).InclusiveBetween(-6, 6);
    RuleFor(x => x.Defense).InclusiveBetween(-6, 6);
    RuleFor(x => x.SpecialAttack).InclusiveBetween(-6, 6);
    RuleFor(x => x.SpecialDefense).InclusiveBetween(-6, 6);
    RuleFor(x => x.Speed).InclusiveBetween(-6, 6);
    RuleFor(x => x.Accuracy).InclusiveBetween(-6, 6);
    RuleFor(x => x.Evasion).InclusiveBetween(-6, 6);
    RuleFor(x => x.Critical).InclusiveBetween(0, 4);
  } // TASK: [POKEGAME-287](https://logitar.atlassian.net/browse/POKEGAME-287)
}
