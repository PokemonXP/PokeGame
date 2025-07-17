namespace PokeGame.Core.Trainers;

public interface ITrainerRepository
{
  Task<Trainer?> LoadAsync(TrainerId trainerId, CancellationToken cancellationToken = default);

  Task SaveAsync(Trainer trainer, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Trainer> trainers, CancellationToken cancellationToken = default);
}
