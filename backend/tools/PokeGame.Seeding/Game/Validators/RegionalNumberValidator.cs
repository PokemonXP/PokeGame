using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class RegionalNumberValidator : AbstractValidator<RegionalNumberPayload>
{
  public RegionalNumberValidator()
  {
    RuleFor(x => x.Region).NotEmpty();
    RuleFor(x => x.Number).InclusiveBetween(1, 9999);
  }
}
