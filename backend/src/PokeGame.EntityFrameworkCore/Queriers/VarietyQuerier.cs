using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Varieties;
using PokeGame.Core.Varieties.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class VarietyQuerier : IVarietyQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<VarietyEntity> _varieties;
  private readonly ISqlHelper _sqlHelper;

  public VarietyQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _varieties = context.Varieties;
    _sqlHelper = sqlHelper;
  }

  public async Task<VarietyModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _varieties.AsNoTracking()
      .Include(x => x.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return variety is null ? null : await MapAsync(variety, cancellationToken);
  }
  public async Task<VarietyModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    VarietyEntity? variety = await _varieties.AsNoTracking()
      .Include(x => x.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return variety is null ? null : await MapAsync(variety, cancellationToken);
  }

  public async Task<SearchResults<VarietyModel>> SearchAsync(Guid speciesId, SearchVarietiesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Varieties.Table).SelectAll(PokemonDb.Varieties.Table)
      .Where(PokemonDb.Varieties.SpeciesUid, Operators.IsEqualTo(speciesId))
      .ApplyIdFilter(PokemonDb.Varieties.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Varieties.UniqueName, PokemonDb.Varieties.DisplayName, PokemonDb.Varieties.Genus);

    IQueryable<VarietyEntity> query = _varieties.FromQuery(builder).AsNoTracking()
      .Include(x => x.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region);
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<VarietyEntity>? ordered = null;
    foreach (VarietySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case VarietySort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case VarietySort.DisplayName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case VarietySort.UniqueName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case VarietySort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    VarietyEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<VarietyModel> varieties = await MapAsync(entities, cancellationToken);

    return new SearchResults<VarietyModel>(varieties, total);
  }

  private async Task<VarietyModel> MapAsync(VarietyEntity variety, CancellationToken cancellationToken)
  {
    return (await MapAsync([variety], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<VarietyModel>> MapAsync(IEnumerable<VarietyEntity> varieties, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = varieties.SelectMany(variety => variety.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return varieties.Select(mapper.ToVariety).ToList().AsReadOnly();
  }
}
