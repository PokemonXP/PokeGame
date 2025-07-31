using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Battles;
using PokeGame.Core.Battles.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class BattleQuerier : IBattleQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<BattleEntity> _battles;
  private readonly DbSet<PokemonEntity> _pokemon;
  private readonly ISqlHelper _sqlHelper;

  public BattleQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _battles = context.Battles;
    _pokemon = context.Pokemon;
    _sqlHelper = sqlHelper;
  }

  public async Task<BattleModel> ReadAsync(Battle battle, CancellationToken cancellationToken)
  {
    return await ReadAsync(battle.Id, cancellationToken) ?? throw new InvalidOperationException($"The battle entity 'StreamId={battle.Id}' was not found.");
  }
  public async Task<BattleModel?> ReadAsync(BattleId id, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _battles.AsNoTracking()
      .Include(x => x.Pokemon)
      .Include(x => x.Trainers).ThenInclude(x => x.Trainer)
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    await FillAsync(battle, cancellationToken);
    return await MapAsync(battle, cancellationToken);
  }
  public async Task<BattleModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _battles.AsNoTracking()
      .Include(x => x.Pokemon)
      .Include(x => x.Trainers).ThenInclude(x => x.Trainer)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    await FillAsync(battle, cancellationToken);
    return await MapAsync(battle, cancellationToken);
  }

  public async Task<SearchResults<BattleModel>> SearchAsync(SearchBattlesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Battles.Table).SelectAll(PokemonDb.Battles.Table)
      .ApplyIdFilter(PokemonDb.Battles.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Battles.Name, PokemonDb.Battles.Location);

    if (payload.Kind.HasValue)
    {
      builder.Where(PokemonDb.Battles.Kind, Operators.IsEqualTo(payload.Kind.Value.ToString()));
    }
    if (payload.Status.HasValue)
    {
      builder.Where(PokemonDb.Battles.Status, Operators.IsEqualTo(payload.Status.Value.ToString()));
    }
    if (payload.TrainerId.HasValue)
    {
      builder.Join(PokemonDb.BattleTrainers.BattleId, PokemonDb.Battles.BattleId)
        .Where(PokemonDb.BattleTrainers.TrainerUid, Operators.IsEqualTo(payload.TrainerId.Value));
    }

    IQueryable<BattleEntity> query = _battles.FromQuery(builder).AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<BattleEntity>? ordered = null;
    foreach (BattleSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case BattleSort.CancelledOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CancelledOn) : query.OrderBy(x => x.CancelledOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CancelledOn) : ordered.ThenBy(x => x.CancelledOn));
          break;
        case BattleSort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case BattleSort.Location:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Location) : query.OrderBy(x => x.Location))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Location) : ordered.ThenBy(x => x.Location));
          break;
        case BattleSort.Name:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case BattleSort.StartedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.StartedOn) : query.OrderBy(x => x.StartedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.StartedOn) : ordered.ThenBy(x => x.StartedOn));
          break;
        case BattleSort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    BattleEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<BattleModel> battles = await MapAsync(entities, cancellationToken);

    return new SearchResults<BattleModel>(battles, total);
  }

  private async Task FillAsync(BattleEntity battle, CancellationToken cancellationToken)
  {
    if (battle.Pokemon.Count > 0)
    {
      HashSet<int> pokemonIds = battle.Pokemon.Select(x => x.PokemonId).ToHashSet();
      Dictionary<int, PokemonEntity> pokemon = await _pokemon.AsNoTracking()
        .Include(x => x.CurrentTrainer)
        .Include(x => x.Form).ThenInclude(x => x!.Abilities).ThenInclude(x => x.Ability)
        .Include(x => x.Form).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species)
        .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
        .Include(x => x.Moves).ThenInclude(x => x.Move)
        .Include(x => x.OriginalTrainer)
        .Include(x => x.PokeBall)
        .Where(x => pokemonIds.Contains(x.PokemonId))
        .ToDictionaryAsync(x => x.PokemonId, x => x, cancellationToken);
      foreach (BattlePokemonEntity entity in battle.Pokemon)
      {
        entity.Pokemon = pokemon[entity.PokemonId];
      }
    }
  }

  private async Task<BattleModel> MapAsync(BattleEntity battle, CancellationToken cancellationToken)
  {
    return (await MapAsync([battle], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<BattleModel>> MapAsync(IEnumerable<BattleEntity> battles, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = battles.SelectMany(battle => battle.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return battles.Select(mapper.ToBattle).ToList().AsReadOnly();
  }
}
