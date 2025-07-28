using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Evolutions.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class EvolutionQuerier : IEvolutionQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<EvolutionEntity> _evolutions;
  private readonly ISqlHelper _sqlHelper;

  public EvolutionQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _evolutions = context.Evolutions;
    _sqlHelper = sqlHelper;
  }

  public async Task<EvolutionModel> ReadAsync(Evolution evolution, CancellationToken cancellationToken)
  {
    return await ReadAsync(evolution.Id, cancellationToken) ?? throw new InvalidOperationException($"The evolution entity 'StreamId={evolution.Id}' was not found.");
  }
  public async Task<EvolutionModel?> ReadAsync(EvolutionId id, CancellationToken cancellationToken)
  {
    EvolutionEntity? evolution = await _evolutions.AsNoTracking()
      .Include(x => x.Source).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species)
      .Include(x => x.Target).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species)
      .Include(x => x.Item).ThenInclude(x => x!.Move)
      .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
      .Include(x => x.KnownMove)
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    return evolution is null ? null : await MapAsync(evolution, cancellationToken);
  }
  public async Task<EvolutionModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    EvolutionEntity? evolution = await _evolutions.AsNoTracking()
      .Include(x => x.Source).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species)
      .Include(x => x.Target).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species)
      .Include(x => x.Item).ThenInclude(x => x!.Move)
      .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
      .Include(x => x.KnownMove)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return evolution is null ? null : await MapAsync(evolution, cancellationToken);
  }

  public async Task<SearchResults<EvolutionModel>> SearchAsync(SearchEvolutionsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Evolutions.Table).SelectAll(PokemonDb.Evolutions.Table)
      .ApplyIdFilter(PokemonDb.Evolutions.Id, payload.Ids);

    if (payload.SourceId.HasValue)
    {
      builder.Where(PokemonDb.Evolutions.SourceUid, Operators.IsEqualTo(payload.SourceId.Value));
    }
    if (payload.TargetId.HasValue)
    {
      builder.Where(PokemonDb.Evolutions.TargetUid, Operators.IsEqualTo(payload.TargetId.Value));
    }
    if (payload.Trigger.HasValue)
    {
      builder.Where(PokemonDb.Evolutions.Trigger, Operators.IsEqualTo(payload.Trigger.Value.ToString()));
    }

    IQueryable<EvolutionEntity> query = _evolutions.FromQuery(builder).AsNoTracking()
      .Include(x => x.Source).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species)
      .Include(x => x.Target).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species)
      .Include(x => x.Item).ThenInclude(x => x!.Move)
      .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
      .Include(x => x.KnownMove);
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<EvolutionEntity>? ordered = null;
    foreach (EvolutionSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case EvolutionSort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case EvolutionSort.Level:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Level) : query.OrderBy(x => x.Level))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Level) : ordered.ThenBy(x => x.Level));
          break;
        case EvolutionSort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    EvolutionEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<EvolutionModel> evolutions = await MapAsync(entities, cancellationToken);

    return new SearchResults<EvolutionModel>(evolutions, total);
  }

  private async Task<EvolutionModel> MapAsync(EvolutionEntity evolution, CancellationToken cancellationToken)
  {
    return (await MapAsync([evolution], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<EvolutionModel>> MapAsync(IEnumerable<EvolutionEntity> evolutions, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = evolutions.SelectMany(evolution => evolution.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return evolutions.Select(mapper.ToEvolution).ToList().AsReadOnly();
  }
}
