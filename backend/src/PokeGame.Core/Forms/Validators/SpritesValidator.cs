using FluentValidation;
using Krakenar.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms.Validators;

internal class SpritesValidator : AbstractValidator<SpritesModel>
{
  public SpritesValidator()
  {
    RuleFor(x => x.Default).Url().NotEqual(x => x.Shiny).NotEqual(x => x.Alternative).NotEqual(x => x.AlternativeShiny);
    RuleFor(x => x.Shiny).Url().NotEqual(x => x.Default).NotEqual(x => x.Alternative).NotEqual(x => x.AlternativeShiny);
    When(x => !string.IsNullOrWhiteSpace(x.Alternative),
      () => RuleFor(x => x.Alternative!).Url().NotEqual(x => x.Default).NotEqual(x => x.Shiny).NotEqual(x => x.AlternativeShiny));
    When(x => !string.IsNullOrWhiteSpace(x.AlternativeShiny),
      () => RuleFor(x => x.AlternativeShiny!).Url().NotEqual(x => x.Default).NotEqual(x => x.Shiny).NotEqual(x => x.Alternative));
  }
}
