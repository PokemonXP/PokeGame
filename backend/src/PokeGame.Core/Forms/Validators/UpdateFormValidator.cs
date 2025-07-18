using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms.Validators;

internal class UpdateFormValidator : AbstractValidator<UpdateFormPayload>
{
  public UpdateFormValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Height.HasValue, () => RuleFor(x => x.Height!.Value).Height());
    When(x => x.Weight.HasValue, () => RuleFor(x => x.Weight!.Value).Weight());

    When(x => x.Types is not null, () => RuleFor(x => x.Types!).SetValidator(new FormTypesValidator()));
    When(x => x.Abilities is not null, () => RuleFor(x => x.Abilities!).SetValidator(new FormAbilitiesValidator()));
    When(x => x.BaseStatistics is not null, () => RuleFor(x => x.BaseStatistics!).SetValidator(new BaseStatisticsValidator()));
    When(x => x.Yield is not null, () => RuleFor(x => x.Yield!).SetValidator(new YieldValidator()));
    When(x => x.Sprites is not null, () => RuleFor(x => x.Sprites!).SetValidator(new SpritesValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
  }
}
