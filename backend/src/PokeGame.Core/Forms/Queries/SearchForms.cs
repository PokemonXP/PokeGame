using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms.Queries;

internal record SearchForms(SearchFormsPayload Payload) : IQuery<SearchResults<FormModel>>;

internal class SearchFormsHandler : IQueryHandler<SearchForms, SearchResults<FormModel>>
{
  private readonly IFormQuerier _formQuerier;

  public SearchFormsHandler(IFormQuerier formQuerier)
  {
    _formQuerier = formQuerier;
  }

  public async Task<SearchResults<FormModel>> HandleAsync(SearchForms query, CancellationToken cancellationToken)
  {
    return await _formQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
