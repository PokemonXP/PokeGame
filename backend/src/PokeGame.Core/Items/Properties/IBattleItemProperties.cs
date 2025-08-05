using PokeGame.Core.Battles;

namespace PokeGame.Core.Items.Properties;

public interface IBattleItemProperties : IStatisticChanges
{
  int GuardTurns { get; }
}
