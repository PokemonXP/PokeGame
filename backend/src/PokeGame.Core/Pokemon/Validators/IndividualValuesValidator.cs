using FluentValidation;

namespace PokeGame.Core.Pokemon.Validators;

internal class IndividualValuesValidator : AbstractValidator<IIndividualValues>
{
  public IndividualValuesValidator()
  {
    RuleFor(x => x.HP).LessThanOrEqualTo(IndividualValues.MaximumValue);
    RuleFor(x => x.Attack).LessThanOrEqualTo(IndividualValues.MaximumValue);
    RuleFor(x => x.Defense).LessThanOrEqualTo(IndividualValues.MaximumValue);
    RuleFor(x => x.SpecialAttack).LessThanOrEqualTo(IndividualValues.MaximumValue);
    RuleFor(x => x.SpecialDefense).LessThanOrEqualTo(IndividualValues.MaximumValue);
    RuleFor(x => x.Speed).LessThanOrEqualTo(IndividualValues.MaximumValue);
  }
}
