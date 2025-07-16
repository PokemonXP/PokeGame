using FluentValidation;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Core.Pokemons.Validators;

internal class SwitchPokemonMovesValidator : AbstractValidator<SwitchPokemonMovesPayload>
{
  public SwitchPokemonMovesValidator()
  {
    RuleFor(x => x.Source).GreaterThanOrEqualTo(0).LessThan(Pokemon.MoveLimit).NotEqual(x => x.Destination);
    RuleFor(x => x.Destination).GreaterThanOrEqualTo(0).LessThan(Pokemon.MoveLimit).NotEqual(x => x.Source);
  }
}
