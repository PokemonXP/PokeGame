using FluentValidation;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Validators;

internal class VictoriousPokemonValidator : AbstractValidator<VictoriousPokemonPayload>
{
  public VictoriousPokemonValidator()
  {
    RuleFor(x => x.Pokemon).NotEmpty();
    RuleFor(x => x.Experience).GreaterThan(0);
  }
}
