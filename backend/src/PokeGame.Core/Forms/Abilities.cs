using FluentValidation;
using PokeGame.Core.Abilities;

namespace PokeGame.Core.Forms;

public record Abilities
{
  public AbilityId Primary { get; }
  public AbilityId? Secondary { get; }
  public AbilityId? Hidden { get; }

  public Abilities(AbilityId primary, AbilityId? secondary = null, AbilityId? hidden = null)
  {
    Primary = primary;
    Secondary = secondary;
    Hidden = hidden;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<Abilities>
  {
    public Validator()
    {
      When(x => x.Secondary.HasValue, () =>
      {
        RuleFor(x => x.Primary).NotEqual(x => x.Secondary!.Value);
        RuleFor(x => x.Hidden).NotEqual(x => x.Secondary);
      });
      When(x => x.Hidden.HasValue, () =>
      {
        RuleFor(x => x.Primary).NotEqual(x => x.Hidden!.Value);
        RuleFor(x => x.Secondary).NotEqual(x => x.Hidden);
      });
    }
  }
}
