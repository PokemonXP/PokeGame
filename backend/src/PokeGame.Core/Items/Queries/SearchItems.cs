using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items.Queries;

internal record SearchItems(SearchItemsPayload Payload) : IQuery<SearchResults<ItemModel>>;

internal class SearchItemsHandler : IQueryHandler<SearchItems, SearchResults<ItemModel>>
{
  private readonly IItemQuerier _itemQuerier;

  public SearchItemsHandler(IItemQuerier itemQuerier)
  {
    _itemQuerier = itemQuerier;
  }

  public async Task<SearchResults<ItemModel>> HandleAsync(SearchItems query, CancellationToken cancellationToken)
  {
    return await _itemQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
