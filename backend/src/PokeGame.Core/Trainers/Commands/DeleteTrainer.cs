using Krakenar.Core;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Core.Trainers.Commands;

internal record DeleteTrainer(Guid Id) : ICommand<TrainerModel?>;

internal class DeleteTrainerHandler : ICommandHandler<DeleteTrainer, TrainerModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITrainerQuerier _trainerQuerier;
  private readonly ITrainerRepository _trainerRepository;

  public DeleteTrainerHandler(IApplicationContext applicationContext, ITrainerQuerier trainerQuerier, ITrainerRepository trainerRepository)
  {
    _applicationContext = applicationContext;
    _trainerQuerier = trainerQuerier;
    _trainerRepository = trainerRepository;
  }

  public async Task<TrainerModel?> HandleAsync(DeleteTrainer command, CancellationToken cancellationToken)
  {
    TrainerId trainerId = new(command.Id);
    Trainer? trainer = await _trainerRepository.LoadAsync(trainerId, cancellationToken);
    if (trainer is null)
    {
      return null;
    }
    TrainerModel model = await _trainerQuerier.ReadAsync(trainer, cancellationToken);

    trainer.Delete(_applicationContext.ActorId);
    await _trainerRepository.SaveAsync(trainer, cancellationToken);

    return model;
  }
}
