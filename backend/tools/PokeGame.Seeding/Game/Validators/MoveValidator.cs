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

    RuleFor(x => x.Accuracy).InclusiveBetween(30, 100);
    When(x => x.Category == MoveCategory.Special, () => RuleFor(x => x.Power).Null())
      .Otherwise(() => RuleFor(x => x.Power).InclusiveBetween(10, 250));
    RuleFor(x => x.PowerPoints).InclusiveBetween(1, 40);

    When(x => x.InflictedStatus is not null, () => RuleFor(x => x.InflictedStatus!).SetValidator(new InflictedStatusValidator()));
    RuleForEach(x => x.StatisticChanges).SetValidator(new StatisticChangeValidator());
    RuleForEach(x => x.VolatileConditions).IsInEnum();

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
  }
}
