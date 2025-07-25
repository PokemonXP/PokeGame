using PokeGame.Core.Inventory;

namespace PokeGame.Core.Trainers;

public interface ITrainerRepository
{
  Task<Trainer?> LoadAsync(TrainerId trainerId, CancellationToken cancellationToken = default);
  Task<Trainer?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken = default);

  Task<TrainerInventory> LoadInventoryAsync(Trainer trainer, CancellationToken cancellationToken = default);

  Task SaveAsync(Trainer trainer, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Trainer> trainers, CancellationToken cancellationToken = default);

  Task SaveAsync(TrainerInventory inventory, CancellationToken cancellationToken = default);
}
