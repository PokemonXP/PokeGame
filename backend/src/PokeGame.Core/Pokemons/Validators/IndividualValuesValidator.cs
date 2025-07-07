using FluentValidation;

namespace PokeGame.Core.Pokemons.Validators;

internal class IndividualValuesValidator : AbstractValidator<IIndividualValues>
{
  public IndividualValuesValidator()
  {
    RuleFor(x => x.HP).InclusiveBetween((byte)0, (byte)31);
    RuleFor(x => x.Attack).InclusiveBetween((byte)0, (byte)31);
    RuleFor(x => x.Defense).InclusiveBetween((byte)0, (byte)31);
    RuleFor(x => x.SpecialAttack).InclusiveBetween((byte)0, (byte)31);
    RuleFor(x => x.SpecialDefense).InclusiveBetween((byte)0, (byte)31);
    RuleFor(x => x.Speed).InclusiveBetween((byte)0, (byte)31);
  }
}
