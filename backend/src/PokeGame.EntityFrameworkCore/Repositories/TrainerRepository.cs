using Logitar.EventSourcing;
using PokeGame.Core.Trainers;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class TrainerRepository : Repository, ITrainerRepository
{
  public TrainerRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Trainer?> LoadAsync(TrainerId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Trainer>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Trainer trainer, CancellationToken cancellationToken)
  {
    await base.SaveAsync(trainer, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Trainer> trainer, CancellationToken cancellationToken)
  {
    await base.SaveAsync(trainer, cancellationToken);
  }
}
