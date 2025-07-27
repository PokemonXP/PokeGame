using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Evolutions.Models;

namespace PokeGame.Core.Evolutions.Queries;

internal record SearchEvolutions(SearchEvolutionsPayload Payload) : IQuery<SearchResults<EvolutionModel>>;

internal class SearchEvolutionsHandler : IQueryHandler<SearchEvolutions, SearchResults<EvolutionModel>>
{
  private readonly IEvolutionQuerier _evolutionQuerier;

  public SearchEvolutionsHandler(IEvolutionQuerier evolutionQuerier)
  {
    _evolutionQuerier = evolutionQuerier;
  }

  public async Task<SearchResults<EvolutionModel>> HandleAsync(SearchEvolutions query, CancellationToken cancellationToken)
  {
    return await _evolutionQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
