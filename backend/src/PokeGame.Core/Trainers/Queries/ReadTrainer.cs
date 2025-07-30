using Krakenar.Contracts;
using Krakenar.Core;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Core.Trainers.Queries;

internal record ReadTrainer(Guid? Id, string? UniqueName, string? License) : IQuery<TrainerModel?>;

/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadTrainerHandler : IQueryHandler<ReadTrainer, TrainerModel?>
{
  private readonly ITrainerQuerier _trainerQuerier;

  public ReadTrainerHandler(ITrainerQuerier trainerQuerier)
  {
    _trainerQuerier = trainerQuerier;
  }

  public async Task<TrainerModel?> HandleAsync(ReadTrainer query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, TrainerModel> trainers = new(capacity: 3);

    if (query.Id.HasValue)
    {
      var trainer = await _trainerQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (trainer is not null)
      {
        trainers[trainer.Id] = trainer;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      var trainer = await _trainerQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (trainer is not null)
      {
        trainers[trainer.Id] = trainer;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.License))
    {
      var trainer = await _trainerQuerier.ReadByLicenseAsync(query.License, cancellationToken);
      if (trainer is not null)
      {
        trainers[trainer.Id] = trainer;
      }
    }

    if (trainers.Count > 1)
    {
      throw TooManyResultsException<TrainerModel>.ExpectedSingle(trainers.Count);
    }

    return trainers.SingleOrDefault().Value;
  }
}
