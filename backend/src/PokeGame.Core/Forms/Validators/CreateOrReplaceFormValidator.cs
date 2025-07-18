using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms.Validators;

internal class CreateOrReplaceFormValidator : AbstractValidator<CreateOrReplaceFormPayload>
{
  public CreateOrReplaceFormValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.Variety).NotEmpty();

    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Height).Height();
    RuleFor(x => x.Weight).Weight();

    RuleFor(x => x.Types).SetValidator(new FormTypesValidator());
    RuleFor(x => x.Abilities).SetValidator(new FormAbilitiesValidator());
    RuleFor(x => x.BaseStatistics).SetValidator(new BaseStatisticsValidator());
    RuleFor(x => x.Yield).SetValidator(new YieldValidator());
    RuleFor(x => x.Sprites).SetValidator(new SpritesValidator());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
  }
}
