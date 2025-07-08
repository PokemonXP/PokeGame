using PokeGame.Core.Models;

namespace PokeGame.Core.Items.Models;

public record BattleItemModel
{
  public StatisticChangesModel StatisticChanges { get; set; } = new();
  public int GuardTurns { get; set; }

  public BattleItemModel()
  {
  }

  public BattleItemModel(StatisticChangesModel statisticChanges, int guardTurns)
  {
    StatisticChanges = statisticChanges;
    GuardTurns = guardTurns;
  }
}
