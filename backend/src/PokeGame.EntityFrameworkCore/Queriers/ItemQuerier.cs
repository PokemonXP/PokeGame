using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class ItemQuerier : IItemQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ItemEntity> _items;
  private readonly ISqlHelper _sqlHelper;

  public ItemQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _items = context.Items;
    _sqlHelper = sqlHelper;
  }

  public async Task<ItemId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _items.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    return string.IsNullOrWhiteSpace(streamId) ? null : new ItemId(streamId);
  }

  public async Task<ItemModel> ReadAsync(Item item, CancellationToken cancellationToken)
  {
    return await ReadAsync(item.Id, cancellationToken) ?? throw new InvalidOperationException($"The item entity 'StreamId={item.Id}' was not found.");
  }
  public async Task<ItemModel?> ReadAsync(ItemId id, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _items.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    return item is null ? null : await MapAsync(item, cancellationToken);
  }
  public async Task<ItemModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _items.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return item is null ? null : await MapAsync(item, cancellationToken);
  }
  public async Task<ItemModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    ItemEntity? item = await _items.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return item is null ? null : await MapAsync(item, cancellationToken);
  }

  public async Task<SearchResults<ItemModel>> SearchAsync(SearchItemsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Items.Table).SelectAll(PokemonDb.Items.Table)
      .ApplyIdFilter(PokemonDb.Items.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Items.UniqueName, PokemonDb.Items.DisplayName);

    if (payload.Category.HasValue)
    {
      builder.Where(PokemonDb.Items.Category, Operators.IsEqualTo(payload.Category.Value.ToString()));
    }

    IQueryable<ItemEntity> query = _items.FromQuery(builder).AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ItemEntity>? ordered = null;
    foreach (ItemSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ItemSort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ItemSort.DisplayName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case ItemSort.Price:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Price) : ordered.ThenBy(x => x.Price));
          break;
        case ItemSort.UniqueName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case ItemSort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ItemEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ItemModel> items = await MapAsync(entities, cancellationToken);

    return new SearchResults<ItemModel>(items, total);
  }

  private async Task<ItemModel> MapAsync(ItemEntity item, CancellationToken cancellationToken)
  {
    return (await MapAsync([item], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ItemModel>> MapAsync(IEnumerable<ItemEntity> items, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = items.SelectMany(item => item.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return items.Select(mapper.ToItem).ToList().AsReadOnly();
  }
}
