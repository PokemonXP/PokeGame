using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class BaseStatisticsValidator : AbstractValidator<BaseStatisticsPayload>
{
  public BaseStatisticsValidator()
  {
    RuleFor(x => x.HP).InclusiveBetween(1, 255);
    RuleFor(x => x.Attack).InclusiveBetween(1, 255);
    RuleFor(x => x.Defense).InclusiveBetween(1, 255);
    RuleFor(x => x.SpecialAttack).InclusiveBetween(1, 255);
    RuleFor(x => x.SpecialDefense).InclusiveBetween(1, 255);
    RuleFor(x => x.Speed).InclusiveBetween(1, 255);
  }
}
