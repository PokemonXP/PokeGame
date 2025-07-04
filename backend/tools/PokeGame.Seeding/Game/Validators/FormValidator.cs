using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class FormValidator : AbstractValidator<FormPayload>
{
  public FormValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Variety).NotEmpty();

    RuleFor(x => x.Height).InclusiveBetween(1, 145);
    RuleFor(x => x.Weight).InclusiveBetween(1, 9999);

    RuleFor(x => x.Types).NotNull().SetValidator(new TypesValidator());
    RuleFor(x => x.Abilities).NotNull().SetValidator(new AbilitiesValidator());
    RuleFor(x => x.BaseStatistics).NotNull().SetValidator(new BaseStatisticsValidator());
    RuleFor(x => x.Yield).NotNull().SetValidator(new YieldValidator());
    RuleFor(x => x.Sprites).NotNull().SetValidator(new SpritesValidator());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
  }
}
