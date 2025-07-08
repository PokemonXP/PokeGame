using PokeGame.Core.Models;

namespace PokeGame.Core.Items.Models;

public record BerryModel
{
  public int Healing { get; set; }
  public bool IsHealingPercentage { get; set; }

  public StatusCondition? StatusCondition { get; set; }
  public bool CureConfusion { get; set; }
  public bool CureNonVolatileConditions { get; set; }

  public int PowerPoints { get; set; }

  public StatisticChangesModel StatisticChanges { get; set; } = new();

  public PokemonStatistic? LowerEffortValues { get; set; }

  public bool RaiseFriendship { get; set; }
}
