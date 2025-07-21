using Krakenar.Contracts;
using Krakenar.Core;
using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items.Queries;

internal record ReadItem(Guid? Id, string? UniqueName) : IQuery<ItemModel?>;

/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadItemHandler : IQueryHandler<ReadItem, ItemModel?>
{
  private readonly IItemQuerier _itemQuerier;

  public ReadItemHandler(IItemQuerier itemQuerier)
  {
    _itemQuerier = itemQuerier;
  }

  public async Task<ItemModel?> HandleAsync(ReadItem query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ItemModel> items = new(capacity: 2);

    if (query.Id.HasValue)
    {
      ItemModel? item = await _itemQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (item is not null)
      {
        items[item.Id] = item;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      ItemModel? item = await _itemQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (item is not null)
      {
        items[item.Id] = item;
      }
    }

    if (items.Count > 1)
    {
      throw TooManyResultsException<ItemModel>.ExpectedSingle(items.Count);
    }

    return items.SingleOrDefault().Value;
  }
}
