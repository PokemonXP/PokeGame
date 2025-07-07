using Krakenar.Contracts.Search;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Core.Trainers;

public interface ITrainerQuerier
{
  Task<TrainerModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TrainerModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task<TrainerModel?> ReadByLicenseAsync(string license, CancellationToken cancellationToken = default);

  Task<SearchResults<TrainerModel>> SearchAsync(SearchTrainersPayload payload, CancellationToken cancellationToken = default);
}
