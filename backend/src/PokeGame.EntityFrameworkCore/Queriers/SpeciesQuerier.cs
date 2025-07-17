using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class SpeciesQuerier : ISpeciesQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<SpeciesEntity> _species;
  private readonly ISqlHelper _sqlHelper;

  public SpeciesQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _species = context.Species;
    _sqlHelper = sqlHelper;
  }

  public async Task<SpeciesId?> FindIdAsync(Number number, RegionId? regionId, CancellationToken cancellationToken)
  {
    string? streamId = null;
    if (regionId.HasValue)
    {
      streamId = await _species.AsNoTracking()
        .Include(x => x.RegionalNumbers).ThenInclude(x => x.Region)
        .Where(x => x.RegionalNumbers.Any(r => r.Region!.StreamId == regionId.Value.Value && r.Number == number.Value))
        .Select(x => x.StreamId)
        .SingleOrDefaultAsync(cancellationToken);
    }
    else
    {
      streamId = await _species.AsNoTracking()
        .Where(x => x.Number == number.Value)
        .Select(x => x.StreamId)
        .SingleOrDefaultAsync(cancellationToken);
    }
    return string.IsNullOrWhiteSpace(streamId) ? null : new SpeciesId(streamId);
  }
  public async Task<SpeciesId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _species.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    return string.IsNullOrWhiteSpace(streamId) ? null : new SpeciesId(streamId);
  }

  public async Task<SpeciesModel> ReadAsync(PokemonSpecies species, CancellationToken cancellationToken)
  {
    return await ReadAsync(species.Id, cancellationToken) ?? throw new InvalidOperationException($"The species entity 'StreamId={species.Id}' was not found.");
  }
  public async Task<SpeciesModel?> ReadAsync(SpeciesId id, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _species.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    return species is null ? null : await MapAsync(species, cancellationToken);
  }
  public async Task<SpeciesModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _species.AsNoTracking()
      .Include(x => x.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return species is null ? null : await MapAsync(species, cancellationToken);
  }
  public async Task<SpeciesModel?> ReadAsync(int number, Guid? regionId, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _species.AsNoTracking()
      .Include(x => x.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => regionId.HasValue
        ? x.RegionalNumbers.Any(r => r.RegionUid == regionId.Value && r.Number == number)
        : x.Number == number, cancellationToken);
    return species is null ? null : await MapAsync(species, cancellationToken);
  }
  public async Task<SpeciesModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    SpeciesEntity? species = await _species.AsNoTracking()
      .Include(x => x.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return species is null ? null : await MapAsync(species, cancellationToken);
  }

  public async Task<SearchResults<SpeciesModel>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Species.Table).SelectAll(PokemonDb.Species.Table)
      .ApplyIdFilter(PokemonDb.Species.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Species.UniqueName, PokemonDb.Species.DisplayName);

    if (payload.Category.HasValue)
    {
      builder.Where(PokemonDb.Species.Category, Operators.IsEqualTo(payload.Category.Value.ToString()));
    }
    if (payload.EggGroup.HasValue)
    {
      builder.WhereOr(
        new OperatorCondition(PokemonDb.Species.PrimaryEggGroup, Operators.IsEqualTo(payload.EggGroup.Value.ToString())),
        new OperatorCondition(PokemonDb.Species.SecondaryEggGroup, Operators.IsEqualTo(payload.EggGroup.Value.ToString())));
    }
    if (payload.GrowthRate.HasValue)
    {
      builder.Where(PokemonDb.Species.GrowthRate, Operators.IsEqualTo(payload.GrowthRate.Value.ToString()));
    }
    if (payload.RegionId.HasValue)
    {
      builder.Join(PokemonDb.RegionalNumbers.SpeciesId, PokemonDb.Species.SpeciesId)
        .Where(PokemonDb.RegionalNumbers.RegionUid, Operators.IsEqualTo(payload.RegionId.Value));
    }

    IQueryable<SpeciesEntity> query = _species.FromQuery(builder).AsNoTracking()
      .Include(x => x.RegionalNumbers).ThenInclude(x => x.Region);
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<SpeciesEntity>? ordered = null;
    foreach (SpeciesSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case SpeciesSort.BaseFriendship:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.BaseFriendship) : query.OrderBy(x => x.BaseFriendship))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.BaseFriendship) : ordered.ThenBy(x => x.BaseFriendship));
          break;
        case SpeciesSort.CatchRate:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CatchRate) : query.OrderBy(x => x.CatchRate))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CatchRate) : ordered.ThenBy(x => x.CatchRate));
          break;
        case SpeciesSort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case SpeciesSort.DisplayName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case SpeciesSort.EggCycles:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EggCycles) : query.OrderBy(x => x.EggCycles))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EggCycles) : ordered.ThenBy(x => x.EggCycles));
          break;
        case SpeciesSort.Number:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Number) : ordered.ThenBy(x => x.Number));
          break;
        case SpeciesSort.UniqueName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case SpeciesSort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    SpeciesEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<SpeciesModel> species = await MapAsync(entities, cancellationToken);

    return new SearchResults<SpeciesModel>(species, total);
  }

  private async Task<SpeciesModel> MapAsync(SpeciesEntity species, CancellationToken cancellationToken)
  {
    return (await MapAsync([species], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<SpeciesModel>> MapAsync(IEnumerable<SpeciesEntity> species, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = species.SelectMany(species => species.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return species.Select(mapper.ToSpecies).ToList().AsReadOnly();
  }
}
