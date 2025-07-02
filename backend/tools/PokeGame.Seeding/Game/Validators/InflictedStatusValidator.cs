using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class InflictedStatusValidator : AbstractValidator<InflictedStatusPayload>
{
  public InflictedStatusValidator()
  {
    RuleFor(x => x.Condition).IsInEnum();
    RuleFor(x => x.Chance).InclusiveBetween(1, 100);
  }
}
