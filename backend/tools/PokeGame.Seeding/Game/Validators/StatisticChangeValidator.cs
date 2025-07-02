using FluentValidation;
using PokeGame.Core;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class StatisticChangeValidator : AbstractValidator<StatisticChangePayload>
{
  public StatisticChangeValidator()
  {
    RuleFor(x => x.Statistic).IsInEnum().NotEqual(PokemonStatistic.HP);
    RuleFor(x => x.Stages).InclusiveBetween(-6, 6).NotEqual(0);
  }
}
