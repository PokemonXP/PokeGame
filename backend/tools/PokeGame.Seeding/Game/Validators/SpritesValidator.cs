using FluentValidation;
using Krakenar.Core;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class SpritesValidator : AbstractValidator<SpritesPayload>
{
  public SpritesValidator()
  {
    RuleFor(x => x.Default).Url();
    RuleFor(x => x.DefaultShiny).Url();
    When(x => !string.IsNullOrWhiteSpace(x.Alternative), () => RuleFor(x => x.Alternative!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.AlternativeShiny), () => RuleFor(x => x.AlternativeShiny!).Url());
  }
}
