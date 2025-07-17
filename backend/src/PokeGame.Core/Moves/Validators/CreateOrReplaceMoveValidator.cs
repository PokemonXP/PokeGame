using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Moves.Validators;

internal class CreateOrReplaceMoveValidator : AbstractValidator<CreateOrReplaceMovePayload>
{
  public CreateOrReplaceMoveValidator(IUniqueNameSettings uniqueNameSettings)
  {
    // TODO(fpion): a Status move cannot have power!

    RuleFor(x => x.Type).IsInEnum();
    RuleFor(x => x.Category).IsInEnum();

    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => x.Accuracy.HasValue, () => RuleFor(x => x.Accuracy!.Value).Accuracy());
    When(x => x.Power.HasValue, () => RuleFor(x => x.Power!.Value).Power());
    RuleFor(x => x.PowerPoints).PowerPoints();

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
  }
}
