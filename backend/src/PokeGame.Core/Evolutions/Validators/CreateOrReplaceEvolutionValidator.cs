using FluentValidation;
using PokeGame.Core.Evolutions.Models;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Evolutions.Validators;

internal class CreateOrReplaceEvolutionValidator : AbstractValidator<CreateOrReplaceEvolutionPayload>
{
  public CreateOrReplaceEvolutionValidator()
  {
    RuleFor(x => x.Source).NotEmpty().NotEqual(x => x.Target);
    RuleFor(x => x.Target).NotEmpty().NotEqual(x => x.Source);

    RuleFor(x => x.Trigger).IsInEnum();
    When(x => x.Trigger == EvolutionTrigger.Item, () => RuleFor(x => x.Item).NotEmpty())
      .Otherwise(() => RuleFor(x => x.Item).Empty());

    RuleFor(x => x.Level).InclusiveBetween(0, Level.MaximumValue);
    RuleFor(x => x.Gender).IsInEnum();
    When(x => !string.IsNullOrWhiteSpace(x.Location), () => RuleFor(x => x.Location!).Location());
    RuleFor(x => x.TimeOfDay).IsInEnum();
  }
}
