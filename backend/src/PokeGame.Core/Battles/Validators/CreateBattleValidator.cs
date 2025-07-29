using FluentValidation;
using Krakenar.Core;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Validators;

internal class CreateBattleValidator : AbstractValidator<CreateBattlePayload>
{
  public CreateBattleValidator()
  {
    RuleFor(x => x.Kind).IsInEnum();

    RuleFor(x => x.Name).DisplayName();
    RuleFor(x => x.Location).Location();
    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());

    // TODO(fpion): Champions
    // TODO(fpion): Opponents
  }
}
