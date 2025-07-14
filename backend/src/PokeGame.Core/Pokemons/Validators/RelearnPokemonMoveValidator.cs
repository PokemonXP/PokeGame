using FluentValidation;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Core.Pokemons.Validators;

internal class RelearnPokemonMoveValidator : AbstractValidator<RelearnPokemonMovePayload>
{
  public RelearnPokemonMoveValidator()
  {
    RuleFor(x => x.Move).NotEmpty();
    RuleFor(x => x.Position).GreaterThanOrEqualTo(0).LessThan(Pokemon.MoveLimit);
  }
}
