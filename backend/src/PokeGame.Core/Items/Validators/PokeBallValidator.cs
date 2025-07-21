using FluentValidation;
using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Validators;

internal class PokeBallValidator : AbstractValidator<IPokeBallProperties>
{
  public PokeBallValidator()
  {
    RuleFor(x => x.CatchMultiplier).GreaterThan(0.0);
    RuleFor(x => x.FriendshipMultiplier).GreaterThan(0.0);
  }
}
