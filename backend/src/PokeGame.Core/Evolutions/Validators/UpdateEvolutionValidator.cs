using FluentValidation;
using PokeGame.Core.Evolutions.Models;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Evolutions.Validators;

internal class UpdateEvolutionValidator : AbstractValidator<UpdateEvolutionPayload>
{
  public UpdateEvolutionValidator()
  {
    RuleFor(x => x.Level).InclusiveBetween(0, Level.MaximumValue);
    When(x => x.Gender is not null, () => RuleFor(x => x.Gender!.Value).IsInEnum());
    When(x => !string.IsNullOrWhiteSpace(x.Location?.Value), () => RuleFor(x => x.Location!.Value!).Location());
    When(x => x.TimeOfDay is not null, () => RuleFor(x => x.TimeOfDay!.Value).IsInEnum());
  }
}
