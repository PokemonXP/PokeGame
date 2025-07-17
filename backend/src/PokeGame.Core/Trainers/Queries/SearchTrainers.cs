using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Core.Trainers.Queries;

internal record SearchTrainers(SearchTrainersPayload Payload) : IQuery<SearchResults<TrainerModel>>;

internal class SearchTrainersHandler : IQueryHandler<SearchTrainers, SearchResults<TrainerModel>>
{
  private readonly ITrainerQuerier _trainerQuerier;

  public SearchTrainersHandler(ITrainerQuerier trainerQuerier)
  {
    _trainerQuerier = trainerQuerier;
  }

  public async Task<SearchResults<TrainerModel>> HandleAsync(SearchTrainers query, CancellationToken cancellationToken)
  {
    return await _trainerQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
