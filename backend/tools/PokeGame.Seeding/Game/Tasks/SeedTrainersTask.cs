using MediatR;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedTrainersTask : SeedingTask
{
  public override string? Description => "Seeds Trainer contents into Krakenar.";
}

internal class SeedTrainersTaskHandler : INotificationHandler<SeedTrainersTask>
{
  private readonly ILogger<SeedTrainersTaskHandler> _logger;
  private readonly ITrainerService _trainerService;

  public SeedTrainersTaskHandler(ILogger<SeedTrainersTaskHandler> logger, ITrainerService trainerService)
  {
    _logger = logger;
    _trainerService = trainerService;
  }

  public async Task Handle(SeedTrainersTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedTrainerPayload> trainers = await CsvHelper.ExtractAsync<SeedTrainerPayload>("Game/data/trainers.csv", cancellationToken);
    foreach (SeedTrainerPayload trainer in trainers)
    {
      CreateOrReplaceTrainerResult result = await _trainerService.CreateOrReplaceAsync(trainer, trainer.Id, cancellationToken);
      _logger.LogInformation("The trainer '{Trainer}' was {Status}.", result.Trainer, result.Created ? "created" : "updated");
    }
  }
}
