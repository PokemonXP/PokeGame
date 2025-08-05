using PokeGame.Core.Battles;

namespace PokeGame.Core.Items.Properties;

public interface IBerryProperties : IStatisticChanges
{
  int Healing { get; }
  bool IsHealingPercentage { get; }

  StatusCondition? StatusCondition { get; }
  bool AllConditions { get; }
  bool CureConfusion { get; }

  int PowerPoints { get; }

  PokemonStatistic? LowerEffortValues { get; }
  bool RaiseFriendship { get; }
}
