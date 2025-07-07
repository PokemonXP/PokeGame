using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Moves;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class MoveValidator : AbstractValidator<MovePayload>
{
  public MoveValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Type).IsInEnum();
    RuleFor(x => x.Category).IsInEnum();

    RuleFor(x => x.Accuracy).InclusiveBetween(0, 100);
    When(x => x.Category == MoveCategory.Status, () => RuleFor(x => x.Power).Equal(0))
      .Otherwise(() => RuleFor(x => x.Power).InclusiveBetween(0, 250));
    RuleFor(x => x.PowerPoints).InclusiveBetween(1, 40);

    When(x => x.InflictedStatus is not null, () => RuleFor(x => x.InflictedStatus!).SetValidator(new InflictedStatusValidator()));
    RuleFor(x => x.VolatileConditions).Must(BeValidVolatileConditions)
      .WithErrorCode("VolatileConditionsValidator")
      .WithMessage("'{PropertyName}' must be a list of valid volatile conditions separated by pipes (|).");

    RuleFor(x => x.StatisticChanges).SetValidator(new StatisticChangesValidator());
    RuleFor(x => x.CriticalChange).InclusiveBetween(0, 4);

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
  }

  private static bool BeValidVolatileConditions(string? volatileConditions)
  {
    if (string.IsNullOrWhiteSpace(volatileConditions))
    {
      return true;
    }

    return volatileConditions.Split('|').All(value => Enum.TryParse(value, out VolatileCondition volatileCondition) && Enum.IsDefined(volatileCondition));
  }
}
