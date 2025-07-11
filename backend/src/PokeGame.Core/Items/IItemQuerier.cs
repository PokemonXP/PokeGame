using Krakenar.Contracts.Search;
using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items;

public interface IItemQuerier
{
  Task<ItemModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<ItemModel>> SearchAsync(SearchItemsPayload payload, CancellationToken cancellationToken = default);
}
