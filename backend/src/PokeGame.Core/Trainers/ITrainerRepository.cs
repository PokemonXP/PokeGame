using PokeGame.Core.Inventory;
using PokeGame.Core.Storage;

namespace PokeGame.Core.Trainers;

public interface ITrainerRepository
{
  Task<Trainer?> LoadAsync(TrainerId trainerId, CancellationToken cancellationToken = default);
  Task<Trainer?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Trainer>> LoadAsync(IEnumerable<TrainerId> trainerIds, CancellationToken cancellationToken = default);

  Task<TrainerInventory> LoadInventoryAsync(Trainer trainer, CancellationToken cancellationToken = default);
  Task<TrainerInventory> LoadInventoryAsync(TrainerId trainerId, CancellationToken cancellationToken = default);

  Task<PokemonStorage> LoadStorageAsync(Trainer trainer, CancellationToken cancellationToken = default);
  Task<PokemonStorage> LoadStorageAsync(TrainerId trainerId, CancellationToken cancellationToken = default);

  Task SaveAsync(Trainer trainer, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Trainer> trainers, CancellationToken cancellationToken = default);

  Task SaveAsync(TrainerInventory inventory, CancellationToken cancellationToken = default);

  Task SaveAsync(PokemonStorage storage, CancellationToken cancellationToken = default);
}
