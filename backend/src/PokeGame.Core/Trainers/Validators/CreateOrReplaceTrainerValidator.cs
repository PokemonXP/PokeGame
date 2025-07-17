using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Core.Trainers.Validators;

internal class CreateOrReplaceTrainerValidator : AbstractValidator<CreateOrReplaceTrainerPayload>
{
  public CreateOrReplaceTrainerValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.License).License();

    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Gender).IsInEnum();
    RuleFor(x => x.Money).Money();

    When(x => !string.IsNullOrWhiteSpace(x.Sprite), () => RuleFor(x => x.Sprite!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
  }
}
