using FluentValidation;
using PokeGame.Core.Abilities;

namespace PokeGame.Core.Forms;

public record FormAbilities
{
  public AbilityId Primary { get; }
  public AbilityId? Secondary { get; }
  public AbilityId? Hidden { get; }

  public FormAbilities(Ability primary, Ability? secondary = null, Ability? hidden = null)
    : this(primary.Id, secondary?.Id, hidden?.Id)
  {
  }

  [JsonConstructor]
  public FormAbilities(AbilityId primary, AbilityId? secondary = null, AbilityId? hidden = null)
  {
    Primary = primary;
    Secondary = secondary;
    Hidden = hidden;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<FormAbilities>
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
