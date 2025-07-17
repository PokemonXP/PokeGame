using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Varieties.Queries;

internal record SearchVarieties(SearchVarietiesPayload Payload) : IQuery<SearchResults<VarietyModel>>;

internal class SearchVarietiesHandler : IQueryHandler<SearchVarieties, SearchResults<VarietyModel>>
{
  private readonly IVarietyQuerier _varietyQuerier;

  public SearchVarietiesHandler(IVarietyQuerier varietyQuerier)
  {
    _varietyQuerier = varietyQuerier;
  }

  public async Task<SearchResults<VarietyModel>> HandleAsync(SearchVarieties query, CancellationToken cancellationToken)
  {
    return await _varietyQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
