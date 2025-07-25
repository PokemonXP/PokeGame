using FluentValidation;

namespace PokeGame.Core.Pokemon.Validators;

internal class EffortValuesValidator : AbstractValidator<IEffortValues>
{
  private const int Limit = 510;

  public EffortValuesValidator()
  {
    RuleFor(x => x).Must(BeValidEffortValues)
      .WithErrorCode("EffortValuesValidator")
      .WithMessage($"The sum of Effort Values must be less than or equal to {Limit}.");
  }

  private static bool BeValidEffortValues(IEffortValues effortValues)
  {
    int sum = effortValues.HP + effortValues.Attack + effortValues.Defense + effortValues.SpecialAttack + effortValues.SpecialDefense + effortValues.Speed;
    return sum <= Limit;
  }
}
