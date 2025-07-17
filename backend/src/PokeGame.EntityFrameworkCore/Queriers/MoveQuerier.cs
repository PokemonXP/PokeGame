using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class MoveQuerier : IMoveQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<MoveEntity> _moves;
  private readonly ISqlHelper _sqlHelper;

  public MoveQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _moves = context.Moves;
    _sqlHelper = sqlHelper;
  }

  public async Task<MoveId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _moves.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    return string.IsNullOrWhiteSpace(streamId) ? null : new MoveId(streamId);
  }

  public async Task<MoveModel> ReadAsync(Move move, CancellationToken cancellationToken)
  {
    return await ReadAsync(move.Id, cancellationToken) ?? throw new InvalidOperationException($"The move entity 'StreamId={move.Id}' was not found.");
  }
  public async Task<MoveModel?> ReadAsync(MoveId id, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _moves.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    return move is null ? null : await MapAsync(move, cancellationToken);
  }

  public async Task<MoveModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _moves.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return move is null ? null : await MapAsync(move, cancellationToken);
  }
  public async Task<MoveModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    MoveEntity? move = await _moves.AsNoTracking().SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return move is null ? null : await MapAsync(move, cancellationToken);
  }

  public async Task<SearchResults<MoveModel>> SearchAsync(SearchMovesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Moves.Table).SelectAll(PokemonDb.Moves.Table)
      .ApplyIdFilter(PokemonDb.Moves.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Moves.UniqueName, PokemonDb.Moves.DisplayName);

    if (payload.Type.HasValue)
    {
      builder.Where(PokemonDb.Moves.Type, Operators.IsEqualTo(payload.Type.Value.ToString()));
    }
    if (payload.Category.HasValue)
    {
      builder.Where(PokemonDb.Moves.Category, Operators.IsEqualTo(payload.Category.Value.ToString()));
    }

    IQueryable<MoveEntity> query = _moves.FromQuery(builder).AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<MoveEntity>? ordered = null;
    foreach (MoveSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case MoveSort.Accuracy:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Accuracy) : query.OrderBy(x => x.Accuracy))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Accuracy) : ordered.ThenBy(x => x.Accuracy));
          break;
        case MoveSort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case MoveSort.DisplayName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case MoveSort.Power:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Power) : query.OrderBy(x => x.Power))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Power) : ordered.ThenBy(x => x.Power));
          break;
        case MoveSort.PowerPoints:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PowerPoints) : query.OrderBy(x => x.PowerPoints))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PowerPoints) : ordered.ThenBy(x => x.PowerPoints));
          break;
        case MoveSort.UniqueName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case MoveSort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    MoveEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<MoveModel> moves = await MapAsync(entities, cancellationToken);

    return new SearchResults<MoveModel>(moves, total);
  }

  private async Task<MoveModel> MapAsync(MoveEntity move, CancellationToken cancellationToken)
  {
    return (await MapAsync([move], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<MoveModel>> MapAsync(IEnumerable<MoveEntity> moves, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = moves.SelectMany(move => move.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return moves.Select(mapper.ToMove).ToList().AsReadOnly();
  }
}
