using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class SpeciesValidator : AbstractValidator<SpeciesPayload>
{
  public SpeciesValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());

    RuleFor(x => x.Number).InclusiveBetween(1, 9999);
    RuleFor(x => x.Category).IsInEnum();

    RuleFor(x => x.BaseFriendship).InclusiveBetween(0, 140);
    RuleFor(x => x.CatchRate).InclusiveBetween(3, 255);
    RuleFor(x => x.GrowthRate).IsInEnum();

    RuleForEach(x => x.RegionalNumbers).SetValidator(new RegionalNumberValidator());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
  }
}
