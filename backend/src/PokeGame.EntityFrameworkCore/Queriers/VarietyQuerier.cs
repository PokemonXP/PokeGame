using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core;
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
  private readonly DbSet<SpeciesEntity> _species;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<VarietyEntity> _varieties;

  public VarietyQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _species = context.Species;
    _sqlHelper = sqlHelper;
    _varieties = context.Varieties;
  }

  public async Task<VarietyId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _varieties.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    return string.IsNullOrWhiteSpace(streamId) ? null : new VarietyId(streamId);
  }

  public async Task<VarietyModel> ReadAsync(Variety variety, CancellationToken cancellationToken)
  {
    return await ReadAsync(variety.Id, cancellationToken) ?? throw new InvalidOperationException($"The variety entity 'StreamId={variety.Id}' was not found.");
  }
  public async Task<VarietyModel?> ReadAsync(VarietyId id, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _varieties.AsNoTracking()
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    if (variety is null)
    {
      return null;
    }
    await FillAsync(variety, cancellationToken);
    return await MapAsync(variety, cancellationToken);
  }
  public async Task<VarietyModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _varieties.AsNoTracking()
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    if (variety is null)
    {
      return null;
    }
    await FillAsync(variety, cancellationToken);
    return await MapAsync(variety, cancellationToken);
  }
  public async Task<VarietyModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    VarietyEntity? variety = await _varieties.AsNoTracking()
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    if (variety is null)
    {
      return null;
    }
    await FillAsync(variety, cancellationToken);
    return await MapAsync(variety, cancellationToken);
  }

  public async Task<SearchResults<VarietyModel>> SearchAsync(SearchVarietiesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Varieties.Table).SelectAll(PokemonDb.Varieties.Table)
      .ApplyIdFilter(PokemonDb.Varieties.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Varieties.UniqueName, PokemonDb.Varieties.DisplayName, PokemonDb.Varieties.Genus);

    if (payload.SpeciesId.HasValue)
    {
      builder.Where(PokemonDb.Varieties.SpeciesUid, Operators.IsEqualTo(payload.SpeciesId.Value));
    }

    IQueryable<VarietyEntity> query = _varieties.FromQuery(builder).AsNoTracking()
      .Include(x => x.Moves).ThenInclude(x => x.Move);
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
    await FillAsync(entities, cancellationToken);
    IReadOnlyCollection<VarietyModel> varieties = await MapAsync(entities, cancellationToken);

    return new SearchResults<VarietyModel>(varieties, total);
  }
  private async Task FillAsync(VarietyEntity variety, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _species.AsNoTracking()
      .Include(x => x.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.SpeciesId == variety.SpeciesId, cancellationToken);
    variety.Species = species;
  }
  private async Task FillAsync(IEnumerable<VarietyEntity> varieties, CancellationToken cancellationToken)
  {
    HashSet<int> speciesIds = varieties.Select(x => x.SpeciesId).ToHashSet();
    Dictionary<int, SpeciesEntity> species = await _species.AsNoTracking()
      .Include(x => x.RegionalNumbers).ThenInclude(x => x.Region)
      .Where(x => speciesIds.Contains(x.SpeciesId))
      .ToDictionaryAsync(x => x.SpeciesId, x => x, cancellationToken);
    foreach (VarietyEntity variety in varieties)
    {
      variety.Species = species[variety.SpeciesId];
    }
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
