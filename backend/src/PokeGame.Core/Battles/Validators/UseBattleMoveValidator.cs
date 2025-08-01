using FluentValidation;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Validators;

internal class UseBattleMoveValidator : AbstractValidator<UseBattleMovePayload>
{
  public UseBattleMoveValidator()
  {
    RuleFor(x => x.Attacker).NotEmpty();
    RuleFor(x => x.Move).NotEmpty();
    RuleFor(x => x.PowerPointCost).PowerPoints();
    RuleFor(x => x.StaminaCost).GreaterThanOrEqualTo(0);

    RuleFor(x => x.Targets).Must(x => x.Count > 0)
      .WithErrorCode("TargetsValidator")
      .WithMessage("'{PropertyName}' must contain at least one target.");
    RuleForEach(x => x.Targets).SetValidator(new BattleMoveTargetValidator());
  }
}
