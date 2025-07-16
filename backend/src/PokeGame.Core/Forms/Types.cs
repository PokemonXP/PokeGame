using FluentValidation;

namespace PokeGame.Core.Forms;

public record Types : ITypes
{
  public PokemonType Primary { get; }
  public PokemonType? Secondary { get; }

  [JsonConstructor]
  public Types(PokemonType primary, PokemonType? secondary = null)
  {
    Primary = primary;
    Secondary = secondary;
    new Validator().ValidateAndThrow(this);
  }

  public Types(ITypes types) : this(types.Primary, types.Secondary)
  {
  }

  private class Validator : AbstractValidator<Types>
  {
    public Validator()
    {
      RuleFor(x => x.Primary).IsInEnum();
      When(x => x.Secondary.HasValue, () =>
      {
        RuleFor(x => x.Primary).NotEqual(x => x.Secondary!.Value);
        RuleFor(x => x.Secondary).IsInEnum().NotEqual(x => x.Primary);
      });
    }
  }
}
