using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Inventory;
using PokeGame.Core.Storage;
using PokeGame.Core.Trainers;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class TrainerRepository : Repository, ITrainerRepository
{
  private readonly DbSet<TrainerEntity> _trainers;

  public TrainerRepository(PokemonContext context, IEventStore eventStore) : base(eventStore)
  {
    _trainers = context.Trainers;
  }

  public async Task<Trainer?> LoadAsync(TrainerId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Trainer>(id.StreamId, cancellationToken);
  }

  public async Task<Trainer?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    TrainerId trainerId;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      trainerId = new(id);
      Trainer? trainer = await LoadAsync(trainerId, cancellationToken);
      if (trainer is not null)
      {
        return trainer;
      }
    }

    string uniqueNameNormalized = Helper.Normalize(idOrUniqueName);
    string? streamId = await _trainers.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(streamId))
    {
      return null;
    }

    trainerId = new(streamId);
    return await LoadAsync(trainerId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Trainer>> LoadAsync(IEnumerable<TrainerId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync<Trainer>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task<TrainerInventory> LoadInventoryAsync(Trainer trainer, CancellationToken cancellationToken)
  {
    return await LoadInventoryAsync(trainer.Id, cancellationToken);
  }
  public async Task<TrainerInventory> LoadInventoryAsync(TrainerId trainerId, CancellationToken cancellationToken)
  {
    return await LoadAsync<TrainerInventory>(new TrainerInventoryId(trainerId).StreamId, cancellationToken) ?? new(trainerId);
  }

  public async Task<PokemonStorage> LoadStorageAsync(Trainer trainer, CancellationToken cancellationToken)
  {
    return await LoadStorageAsync(trainer.Id, cancellationToken);
  }
  public async Task<PokemonStorage> LoadStorageAsync(TrainerId trainerId, CancellationToken cancellationToken)
  {
    return await LoadAsync<PokemonStorage>(new PokemonStorageId(trainerId).StreamId, cancellationToken) ?? new(trainerId);
  }

  public async Task SaveAsync(Trainer trainer, CancellationToken cancellationToken)
  {
    await base.SaveAsync(trainer, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Trainer> trainers, CancellationToken cancellationToken)
  {
    await base.SaveAsync(trainers, cancellationToken);
  }

  public async Task SaveAsync(TrainerInventory inventory, CancellationToken cancellationToken)
  {
    await base.SaveAsync(inventory, cancellationToken);
  }

  public async Task SaveAsync(PokemonStorage storage, CancellationToken cancellationToken)
  {
    await base.SaveAsync(storage, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<PokemonStorage> storages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(storages, cancellationToken);
  }
}
