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

    RuleFor(x => x.Champions).Must(HaveAtLeastOneNonEmptyElement)
      .WithErrorCode("NotEmptyValidator")
      .WithMessage("'{PropertyName}' must contain at least one non-empty element.");
    RuleFor(x => x.Opponents).Must(HaveAtLeastOneNonEmptyElement)
      .WithErrorCode("NotEmptyValidator")
      .WithMessage("'{PropertyName}' must contain at least one non-empty element.");
  }

  private static bool HaveAtLeastOneNonEmptyElement(IEnumerable<string> elements) => elements.Any(e => !string.IsNullOrWhiteSpace(e));
}
