using Logitar.EventSourcing;
using PokeGame.Core.Trainers.Events;

namespace PokeGame.Core.Trainers;

internal interface ITrainerManager
{
  Task SaveAsync(Trainer trainer, CancellationToken cancellationToken = default);
}

internal class TrainerManager : ITrainerManager
{
  private readonly ITrainerQuerier _trainerQuerier;
  private readonly ITrainerRepository _trainerRepository;

  public TrainerManager(ITrainerQuerier trainerQuerier, ITrainerRepository trainerRepository)
  {
    _trainerQuerier = trainerQuerier;
    _trainerRepository = trainerRepository;
  }

  public async Task SaveAsync(Trainer trainer, CancellationToken cancellationToken)
  {
    bool hasLicenseChanged = false;
    bool hasUniqueNameChanged = false;
    foreach (IEvent change in trainer.Changes)
    {
      if (change is TrainerCreated)
      {
        hasLicenseChanged = true;
        hasUniqueNameChanged = true;
      }
      else if (change is TrainerUniqueNameChanged)
      {
        hasUniqueNameChanged = true;
      }
    }

    if (hasLicenseChanged)
    {
      TrainerId? conflictId = await _trainerQuerier.FindIdAsync(trainer.License, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(trainer.Id))
      {
        throw new LicenseAlreadyUsedException(trainer, conflictId.Value);
      }
    }

    if (hasUniqueNameChanged)
    {
      TrainerId? conflictId = await _trainerQuerier.FindIdAsync(trainer.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(trainer.Id))
      {
        throw new UniqueNameAlreadyUsedException(trainer, conflictId.Value);
      }
    }

    await _trainerRepository.SaveAsync(trainer, cancellationToken);
  }
}
