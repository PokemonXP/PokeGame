using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Regions.Models;

namespace PokeGame.Core.Regions.Queries;

internal record SearchRegions(SearchRegionsPayload Payload) : IQuery<SearchResults<RegionModel>>;

internal class SearchRegionsHandler : IQueryHandler<SearchRegions, SearchResults<RegionModel>>
{
  private readonly IRegionQuerier _regionQuerier;

  public SearchRegionsHandler(IRegionQuerier regionQuerier)
  {
    _regionQuerier = regionQuerier;
  }

  public async Task<SearchResults<RegionModel>> HandleAsync(SearchRegions query, CancellationToken cancellationToken)
  {
    return await _regionQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
