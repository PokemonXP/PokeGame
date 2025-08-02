using FluentValidation;
using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Validators;

internal class BattleItemValidator : AbstractValidator<IBattleItemProperties>
{
  public BattleItemValidator()
  {
    RuleFor(x => x.Attack).InclusiveBetween(-6, 6);
    RuleFor(x => x.Defense).InclusiveBetween(-6, 6);
    RuleFor(x => x.SpecialAttack).InclusiveBetween(-6, 6);
    RuleFor(x => x.SpecialDefense).InclusiveBetween(-6, 6);
    RuleFor(x => x.Speed).InclusiveBetween(-6, 6);
    RuleFor(x => x.Accuracy).InclusiveBetween(-6, 6);
    RuleFor(x => x.Evasion).InclusiveBetween(-6, 6);
    RuleFor(x => x.Critical).InclusiveBetween(0, 4);
    RuleFor(x => x.GuardTurns).InclusiveBetween(0, 10);
  } // TASK: [POKEGAME-287](https://logitar.atlassian.net/browse/POKEGAME-287)
}
