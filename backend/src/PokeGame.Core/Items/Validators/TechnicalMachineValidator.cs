using FluentValidation;
using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items.Validators;

internal class TechnicalMachineValidator : AbstractValidator<TechnicalMachinePropertiesPayload>
{
  public TechnicalMachineValidator()
  {
    RuleFor(x => x.Move).NotEmpty();
  }
}
