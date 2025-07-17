using FluentValidation;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms.Validators;

internal class FormAbilitiesValidator : AbstractValidator<FormAbilitiesPayload>
{
  public FormAbilitiesValidator()
  {
    RuleFor(x => x.Primary).NotEmpty();
  }
}
