using FluentValidation;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Validators;

internal class GainBattleExperienceValidator : AbstractValidator<GainBattleExperiencePayload>
{
  public GainBattleExperienceValidator()
  {
    RuleFor(x => x.Defeated).NotEmpty();
    RuleFor(x => x.Victorious).Must(x => x.Count > 0)
      .WithErrorCode("TargetsValidator")
      .WithMessage("'{PropertyName}' must contain at least one victorious battler.");
    RuleForEach(x => x.Victorious).SetValidator(new VictoriousPokemonValidator());
  }
}
