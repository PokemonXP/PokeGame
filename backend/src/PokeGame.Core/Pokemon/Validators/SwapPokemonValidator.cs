using FluentValidation;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Validators;

internal class SwapPokemonValidator : AbstractValidator<SwapPokemonPayload>
{
  public SwapPokemonValidator()
  {
    RuleFor(x => x.Source).NotEmpty();
    RuleFor(x => x.Destination).NotEmpty();
  }
}
