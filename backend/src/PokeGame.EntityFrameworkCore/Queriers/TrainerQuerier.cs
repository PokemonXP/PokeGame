using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class TrainerQuerier : ITrainerQuerier
{
  private readonly IActorService _actorService;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<TrainerEntity> _trainers;

  public TrainerQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _sqlHelper = sqlHelper;
    _trainers = context.Trainers;
  }

  public async Task<TrainerId?> FindIdAsync(License license, CancellationToken cancellationToken)
  {
    string licenseNormalized = Helper.Normalize(license.Value);

    string? streamId = await _trainers.AsNoTracking()
      .Where(x => x.LicenseNormalized == licenseNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    return string.IsNullOrWhiteSpace(streamId) ? null : new TrainerId(streamId);
  }
  public async Task<TrainerId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _trainers.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    return string.IsNullOrWhiteSpace(streamId) ? null : new TrainerId(streamId);
  }

  public async Task<TrainerModel> ReadAsync(Trainer trainer, CancellationToken cancellationToken)
  {
    return await ReadAsync(trainer.Id, cancellationToken) ?? throw new InvalidOperationException($"The trainer entity 'StreamId={trainer.Id}' was not found.");
  }
  public async Task<TrainerModel?> ReadAsync(TrainerId id, CancellationToken cancellationToken)
  {
    TrainerEntity? trainer = await _trainers.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    return trainer is null ? null : await MapAsync(trainer, cancellationToken);
  }

  public async Task<TrainerModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TrainerEntity? trainer = await _trainers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return trainer is null ? null : await MapAsync(trainer, cancellationToken);
  }
  public async Task<TrainerModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    TrainerEntity? trainer = await _trainers.AsNoTracking().SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return trainer is null ? null : await MapAsync(trainer, cancellationToken);
  }
  public async Task<TrainerModel?> ReadByLicenseAsync(string license, CancellationToken cancellationToken)
  {
    string licenseNormalized = Helper.Normalize(license);

    TrainerEntity? trainer = await _trainers.AsNoTracking().SingleOrDefaultAsync(x => x.LicenseNormalized == licenseNormalized, cancellationToken);
    return trainer is null ? null : await MapAsync(trainer, cancellationToken);
  }

  public async Task<SearchResults<TrainerModel>> SearchAsync(SearchTrainersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Trainers.Table).SelectAll(PokemonDb.Trainers.Table)
      .ApplyIdFilter(PokemonDb.Trainers.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Trainers.License, PokemonDb.Trainers.UniqueName, PokemonDb.Trainers.DisplayName);

    if (payload.Gender.HasValue)
    {
      builder.Where(PokemonDb.Trainers.Gender, Operators.IsEqualTo(payload.Gender.Value.ToString()));
    }
    if (payload.UserId.HasValue)
    {
      builder.Where(PokemonDb.Trainers.UserUid, Operators.IsEqualTo(payload.UserId.Value));
    }

    IQueryable<TrainerEntity> query = _trainers.FromQuery(builder).AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<TrainerEntity>? ordered = null;
    foreach (TrainerSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case TrainerSort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case TrainerSort.DisplayName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case TrainerSort.License:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.License) : query.OrderBy(x => x.License))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.License) : ordered.ThenBy(x => x.License));
          break;
        case TrainerSort.Money:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Money) : query.OrderBy(x => x.Money))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Money) : ordered.ThenBy(x => x.Money));
          break;
        case TrainerSort.UniqueName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case TrainerSort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    TrainerEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<TrainerModel> trainers = await MapAsync(entities, cancellationToken);

    return new SearchResults<TrainerModel>(trainers, total);
  }

  private async Task<TrainerModel> MapAsync(TrainerEntity trainer, CancellationToken cancellationToken)
  {
    return (await MapAsync([trainer], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<TrainerModel>> MapAsync(IEnumerable<TrainerEntity> trainers, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = trainers.SelectMany(trainer => trainer.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return trainers.Select(mapper.ToTrainer).ToList().AsReadOnly();
  }
}
