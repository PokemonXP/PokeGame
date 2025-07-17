using Logitar.EventSourcing;
using PokeGame.Core.Varieties;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class VarietyRepository : Repository, IVarietyRepository
{
  public VarietyRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Variety?> LoadAsync(VarietyId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Variety>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Variety variety, CancellationToken cancellationToken)
  {
    await base.SaveAsync(variety, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Variety> varieties, CancellationToken cancellationToken)
  {
    await base.SaveAsync(varieties, cancellationToken);
  }
}
