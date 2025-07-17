using FluentValidation;
using PokeGame.Core.Abilities;

namespace PokeGame.Core.Forms;

public record FormAbilities
{
  public AbilityId Primary { get; }
  public AbilityId? Secondary { get; }
  public AbilityId? Hidden { get; }

  [JsonConstructor]
  public FormAbilities(AbilityId primary, AbilityId? secondary = null, AbilityId? hidden = null)
  {
    Primary = primary;
    Secondary = secondary;
    Hidden = hidden;
    new Validator().ValidateAndThrow(this);
  }

  public FormAbilities(Ability primary, Ability? secondary = null, Ability? hidden = null)
    : this(primary.Id, secondary?.Id, hidden?.Id)
  {
  }

  private class Validator : AbstractValidator<FormAbilities>
  {
    public Validator()
    {
      RuleFor(x => x.Primary.Value).NotEmpty();

      When(x => x.Secondary.HasValue, () =>
      {
        RuleFor(x => x.Secondary!.Value.Value).NotEmpty();
        RuleFor(x => x.Secondary).NotEqual(x => x.Primary).NotEqual(x => x.Hidden);
      });

      When(x => x.Hidden.HasValue, () =>
      {
        RuleFor(x => x.Hidden!.Value.Value).NotEmpty();
        RuleFor(x => x.Hidden).NotEqual(x => x.Primary).NotEqual(x => x.Secondary);
      });
    }
  }
}
