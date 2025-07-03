using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class YieldValidator : AbstractValidator<YieldPayload>
{
  public YieldValidator()
  {
    RuleFor(x => x.Experience).InclusiveBetween(36, 635);

    RuleFor(x => x.HP).InclusiveBetween(0, 3);
    RuleFor(x => x.Attack).InclusiveBetween(0, 3);
    RuleFor(x => x.Defense).InclusiveBetween(0, 3);
    RuleFor(x => x.SpecialAttack).InclusiveBetween(0, 3);
    RuleFor(x => x.SpecialDefense).InclusiveBetween(0, 3);
    RuleFor(x => x.Speed).InclusiveBetween(0, 3);
  }
}
