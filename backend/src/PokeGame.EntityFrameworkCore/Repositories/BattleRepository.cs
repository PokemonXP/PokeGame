using Logitar.EventSourcing;
using PokeGame.Core.Battles;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class BattleRepository : Repository, IBattleRepository
{
  public BattleRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Battle?> LoadAsync(BattleId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Battle>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Battle battle, CancellationToken cancellationToken)
  {
    await base.SaveAsync(battle, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Battle> battles, CancellationToken cancellationToken)
  {
    await base.SaveAsync(battles, cancellationToken);
  }
}
