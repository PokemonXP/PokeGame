using FluentValidation;

namespace PokeGame.Core.Pokemons.Validators;

internal class BaseStatisticsValidator : AbstractValidator<IBaseStatistics>
{
  public BaseStatisticsValidator()
  {
    RuleFor(x => x.HP).GreaterThan((byte)0);
    RuleFor(x => x.Attack).GreaterThan((byte)0);
    RuleFor(x => x.Defense).GreaterThan((byte)0);
    RuleFor(x => x.SpecialAttack).GreaterThan((byte)0);
    RuleFor(x => x.SpecialDefense).GreaterThan((byte)0);
    RuleFor(x => x.Speed).GreaterThan((byte)0);
  }
}
