using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Species.Models;

namespace PokeGame.Core.Species.Validators;

internal class CreateOrReplaceSpeciesValidator : AbstractValidator<CreateOrReplaceSpeciesPayload>
{
  public CreateOrReplaceSpeciesValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.Number).Number();
    RuleFor(x => x.Category).IsInEnum();

    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());

    RuleFor(x => x.CatchRate).CatchRate();
    RuleFor(x => x.GrowthRate).IsInEnum();

    RuleFor(x => x.EggCycles).EggCycles();
    RuleFor(x => x.EggGroups).SetValidator(new EggGroupsValidator());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());

    RuleForEach(x => x.RegionalNumbers).SetValidator(new RegionalNumberValidator());
  }
}
