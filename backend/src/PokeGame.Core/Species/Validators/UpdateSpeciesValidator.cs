using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Species.Models;

namespace PokeGame.Core.Species.Validators;

internal class UpdateSpeciesValidator : AbstractValidator<UpdateSpeciesPayload>
{
  public UpdateSpeciesValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());

    When(x => x.CatchRate.HasValue, () => RuleFor(x => x.CatchRate!.Value).CatchRate());
    RuleFor(x => x.GrowthRate).IsInEnum();

    When(x => x.EggCycles.HasValue, () => RuleFor(x => x.EggCycles!.Value).EggCycles());
    When(x => x.EggGroups is not null, () => RuleFor(x => x.EggGroups!).SetValidator(new EggGroupsValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
  }
}
