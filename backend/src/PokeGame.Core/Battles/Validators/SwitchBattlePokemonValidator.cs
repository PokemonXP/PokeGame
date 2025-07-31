using FluentValidation;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Validators;

internal class SwitchBattlePokemonValidator : AbstractValidator<SwitchBattlePokemonPayload>
{
  public SwitchBattlePokemonValidator()
  {
    RuleFor(x => x.Active).NotEmpty();
    RuleFor(x => x.Inactive).NotEmpty();
  }
}
