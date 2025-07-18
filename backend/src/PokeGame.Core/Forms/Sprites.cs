using FluentValidation;
using Krakenar.Core;

namespace PokeGame.Core.Forms;

public record Sprites
{
  public Url Default { get; }
  public Url Shiny { get; }
  public Url? Alternative { get; }
  public Url? AlternativeShiny { get; }

  public Sprites(Url @default, Url shiny, Url? alternative = null, Url? alternativeShiny = null)
  {
    Default = @default;
    Shiny = shiny;
    Alternative = alternative;
    AlternativeShiny = alternativeShiny;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<Sprites>
  {
    public Validator()
    {
      RuleFor(x => x.Default).NotEqual(x => x.Shiny).NotEqual(x => x.Alternative).NotEqual(x => x.AlternativeShiny);
      RuleFor(x => x.Shiny).NotEqual(x => x.Default).NotEqual(x => x.Alternative).NotEqual(x => x.AlternativeShiny);

      When(x => x.Alternative is not null,
        () => RuleFor(x => x.Alternative).NotEqual(x => x.Default).NotEqual(x => x.Shiny).NotEqual(x => x.AlternativeShiny));
      When(x => x.AlternativeShiny is not null,
        () => RuleFor(x => x.AlternativeShiny).NotEqual(x => x.Default).NotEqual(x => x.Shiny).NotEqual(x => x.Alternative));
    }
  }
}
