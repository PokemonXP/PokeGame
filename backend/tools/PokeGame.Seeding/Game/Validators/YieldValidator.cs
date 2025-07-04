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

    RuleFor(x => x).Must(BeAValidYield)
      .WithErrorCode("YieldValidator")
      .WithMessage("The total Effort Value (EV) yield should vary from 1 to 3.");
  }

  private static bool BeAValidYield(YieldPayload yield)
  {
    int total = yield.HP + yield.Attack + yield.Defense + yield.SpecialAttack + yield.SpecialDefense + yield.Speed;
    return total >= 1 && total <= 3;
  }
}
