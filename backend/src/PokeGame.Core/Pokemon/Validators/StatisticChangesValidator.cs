using FluentValidation;
using PokeGame.Core.Battles;

namespace PokeGame.Core.Pokemon.Validators;

internal class StatisticChangesValidator : AbstractValidator<IStatisticChanges>
{
  public StatisticChangesValidator()
  {
    RuleFor(x => x.Attack).InclusiveBetween(StatisticChanges.MinimumStage, StatisticChanges.MaximumStage);
    RuleFor(x => x.Defense).InclusiveBetween(StatisticChanges.MinimumStage, StatisticChanges.MaximumStage);
    RuleFor(x => x.SpecialAttack).InclusiveBetween(StatisticChanges.MinimumStage, StatisticChanges.MaximumStage);
    RuleFor(x => x.SpecialDefense).InclusiveBetween(StatisticChanges.MinimumStage, StatisticChanges.MaximumStage);
    RuleFor(x => x.Speed).InclusiveBetween(StatisticChanges.MinimumStage, StatisticChanges.MaximumStage);
    RuleFor(x => x.Accuracy).InclusiveBetween(StatisticChanges.MinimumStage, StatisticChanges.MaximumStage);
    RuleFor(x => x.Evasion).InclusiveBetween(StatisticChanges.MinimumStage, StatisticChanges.MaximumStage);
    RuleFor(x => x.Critical).InclusiveBetween(StatisticChanges.MinimumCritical, StatisticChanges.MaximumCritical);
  }
}
