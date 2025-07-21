using FluentValidation;
using Krakenar.Core;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Validators;

internal class ReceivePokemonValidator : AbstractValidator<ReceivePokemonPayload>
{
  public ReceivePokemonValidator()
  {
    RuleFor(x => x.Trainer).NotEmpty();
    RuleFor(x => x.PokeBall).NotEmpty();

    RuleFor(x => x.Level).InclusiveBetween(0, Level.MaximumValue);
    RuleFor(x => x.Location).Location();
    When(x => x.MetOn.HasValue, () => RuleFor(x => x.MetOn!.Value).Past());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }

  public ReceivePokemonValidator(Specimen pokemon)
  {
    RuleFor(x => x.Level).LessThanOrEqualTo(pokemon.Level);
  }
}
