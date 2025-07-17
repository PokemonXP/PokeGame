using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items;

public interface IItemQuerier
{
  Task<ItemId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<ItemModel> ReadAsync(Item item, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(ItemId id, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<ItemModel>> SearchAsync(SearchItemsPayload payload, CancellationToken cancellationToken = default);
}
