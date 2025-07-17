using FluentValidation;

namespace PokeGame.Core.Forms.Validators;

internal class FormTypesValidator : AbstractValidator<IFormTypes>
{
  public FormTypesValidator()
  {
    RuleFor(x => x.Primary).IsInEnum();
    RuleFor(x => x.Secondary).IsInEnum().NotEqual(x => x.Primary);
  }
}
