using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

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
  }
}
