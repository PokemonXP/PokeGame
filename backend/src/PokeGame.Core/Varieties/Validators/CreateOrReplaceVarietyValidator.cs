using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Varieties.Validators;

internal class CreateOrReplaceVarietyValidator : AbstractValidator<CreateOrReplaceVarietyPayload>
{
  public CreateOrReplaceVarietyValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.Species).NotEmpty();

    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());

    When(x => !string.IsNullOrWhiteSpace(x.Genus), () => RuleFor(x => x.Genus!).Genus());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => x.GenderRatio.HasValue, () => RuleFor(x => x.GenderRatio!.Value).GenderRatio());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
  }
}
