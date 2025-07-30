using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles;

public interface IBattleQuerier
{
  Task<BattleModel> ReadAsync(Battle battle, CancellationToken cancellationToken = default);
  Task<BattleModel?> ReadAsync(BattleId id, CancellationToken cancellationToken = default);
  Task<BattleModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
