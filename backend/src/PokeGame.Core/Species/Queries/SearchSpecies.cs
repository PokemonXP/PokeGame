using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Species.Models;

namespace PokeGame.Core.Species.Queries;

internal record SearchSpecies(SearchSpeciesPayload Payload) : IQuery<SearchResults<SpeciesModel>>;

internal class SearchSpeciesHandler : IQueryHandler<SearchSpecies, SearchResults<SpeciesModel>>
{
  private readonly ISpeciesQuerier _speciesQuerier;

  public SearchSpeciesHandler(ISpeciesQuerier speciesQuerier)
  {
    _speciesQuerier = speciesQuerier;
  }

  public async Task<SearchResults<SpeciesModel>> HandleAsync(SearchSpecies query, CancellationToken cancellationToken)
  {
    return await _speciesQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
