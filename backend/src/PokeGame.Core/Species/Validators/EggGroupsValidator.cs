using FluentValidation;

namespace PokeGame.Core.Species.Validators;

internal class EggGroupsValidator : AbstractValidator<IEggGroups>
{
  public EggGroupsValidator()
  {
    RuleFor(x => x.Primary).IsInEnum();
    RuleFor(x => x.Secondary).IsInEnum()
      .NotEqual(x => x.Primary)
      .NotEqual(EggGroup.NoEggsDiscovered)
      .NotEqual(EggGroup.Ditto);
    When(x => x.Primary == EggGroup.NoEggsDiscovered || x.Primary == EggGroup.Ditto, () => RuleFor(x => x.Secondary).Null());
  }
}
