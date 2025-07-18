using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Varieties.Validators;

internal class UpdateVarietyValidator : AbstractValidator<UpdateVarietyPayload>
{
  public UpdateVarietyValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());

    When(x => !string.IsNullOrWhiteSpace(x.Genus?.Value), () => RuleFor(x => x.Genus!.Value!).Genus());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.GenderRatio?.Value is not null, () => RuleFor(x => x.GenderRatio!.Value!.Value).GenderRatio());

    When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());

    RuleForEach(x => x.Moves).SetValidator(new VarietyMoveValidator());
  }
}
