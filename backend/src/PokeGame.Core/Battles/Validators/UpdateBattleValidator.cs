using FluentValidation;
using Krakenar.Core;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Validators;

internal class UpdateBattleValidator : AbstractValidator<UpdateBattlePayload>
{
  public UpdateBattleValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Location), () => RuleFor(x => x.Location!).Location());
    When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
  }
}
