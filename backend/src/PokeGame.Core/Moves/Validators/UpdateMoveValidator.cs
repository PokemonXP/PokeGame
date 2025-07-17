using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Moves.Validators;

internal class UpdateMoveValidator : AbstractValidator<UpdateMovePayload>
{
  public UpdateMoveValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Accuracy?.Value is not null, () => RuleFor(x => x.Accuracy!.Value!.Value).Accuracy());
    When(x => x.Power?.Value is not null, () => RuleFor(x => x.Power!.Value!.Value).Power());
    When(x => x.PowerPoints.HasValue, () => RuleFor(x => x.PowerPoints!.Value).PowerPoints());

    When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
  }

  public UpdateMoveValidator(Move move)
  {
    if (move.Category == MoveCategory.Status)
    {
      RuleFor(x => x.Power).Null()
        .WithErrorCode("StatusMoveValidator")
        .WithMessage($"'{{PropertyName}}' must be null when the move category is {MoveCategory.Status}.");
    }
  }
}
