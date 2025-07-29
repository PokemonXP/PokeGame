namespace PokeGame.Core.Battles;

public interface IBattleRepository
{
  Task<Battle?> LoadAsync(BattleId battleId, CancellationToken cancellationToken = default);

  Task SaveAsync(Battle battle, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Battle> battles, CancellationToken cancellationToken = default);
}
