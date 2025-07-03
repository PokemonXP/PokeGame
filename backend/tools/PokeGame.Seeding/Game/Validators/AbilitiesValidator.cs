using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class AbilitiesValidator : AbstractValidator<AbilitiesPayload>
{
  public AbilitiesValidator()
  {
    RuleFor(x => x.Primary).NotEmpty();
  }
}
