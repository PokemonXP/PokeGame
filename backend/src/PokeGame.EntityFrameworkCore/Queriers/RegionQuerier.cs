using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Regions;
using PokeGame.Core.Regions.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class RegionQuerier : IRegionQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<RegionEntity> _regions;
  private readonly ISqlHelper _sqlHelper;

  public RegionQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _regions = context.Regions;
    _sqlHelper = sqlHelper;
  }

  public async Task<RegionModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RegionEntity? region = await _regions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return region is null ? null : await MapAsync(region, cancellationToken);
  }
  public async Task<RegionModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    RegionEntity? region = await _regions.AsNoTracking().SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return region is null ? null : await MapAsync(region, cancellationToken);
  }

  public async Task<SearchResults<RegionModel>> SearchAsync(SearchRegionsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Regions.Table).SelectAll(PokemonDb.Regions.Table)
      .ApplyIdFilter(PokemonDb.Regions.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Regions.UniqueName, PokemonDb.Regions.DisplayName);

    IQueryable<RegionEntity> query = _regions.FromQuery(builder).AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<RegionEntity>? ordered = null;
    foreach (RegionSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RegionSort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case RegionSort.DisplayName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case RegionSort.UniqueName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case RegionSort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    RegionEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<RegionModel> regions = await MapAsync(entities, cancellationToken);

    return new SearchResults<RegionModel>(regions, total);
  }

  private async Task<RegionModel> MapAsync(RegionEntity region, CancellationToken cancellationToken)
  {
    return (await MapAsync([region], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<RegionModel>> MapAsync(IEnumerable<RegionEntity> regions, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = regions.SelectMany(region => region.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return regions.Select(mapper.ToRegion).ToList().AsReadOnly();
  }
}
