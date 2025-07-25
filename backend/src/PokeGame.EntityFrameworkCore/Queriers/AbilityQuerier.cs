﻿using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Abilities;
using PokeGame.Core.Abilities.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class AbilityQuerier : IAbilityQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<AbilityEntity> _abilities;
  private readonly ISqlHelper _sqlHelper;

  public AbilityQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _abilities = context.Abilities;
    _sqlHelper = sqlHelper;
  }

  public async Task<AbilityId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _abilities.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    return string.IsNullOrWhiteSpace(streamId) ? null : new AbilityId(streamId);
  }

  public async Task<IReadOnlyCollection<AbilityKey>> GetKeysAsync(CancellationToken cancellationToken)
  {
    var keys = await _abilities.AsNoTracking()
      .Select(x => new { x.StreamId, x.Id, x.UniqueName })
      .ToArrayAsync(cancellationToken);
    return keys.Select(key => new AbilityKey(new AbilityId(key.StreamId), key.Id, key.UniqueName)).ToList().AsReadOnly();
  }

  public async Task<AbilityModel> ReadAsync(Ability ability, CancellationToken cancellationToken)
  {
    return await ReadAsync(ability.Id, cancellationToken) ?? throw new InvalidOperationException($"The ability entity 'StreamId={ability.Id}' was not found.");
  }
  public async Task<AbilityModel?> ReadAsync(AbilityId id, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _abilities.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    return ability is null ? null : await MapAsync(ability, cancellationToken);
  }
  public async Task<AbilityModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _abilities.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return ability is null ? null : await MapAsync(ability, cancellationToken);
  }
  public async Task<AbilityModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    AbilityEntity? ability = await _abilities.AsNoTracking().SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return ability is null ? null : await MapAsync(ability, cancellationToken);
  }

  public async Task<SearchResults<AbilityModel>> SearchAsync(SearchAbilitiesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Abilities.Table).SelectAll(PokemonDb.Abilities.Table)
      .ApplyIdFilter(PokemonDb.Abilities.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Abilities.UniqueName, PokemonDb.Abilities.DisplayName);

    IQueryable<AbilityEntity> query = _abilities.FromQuery(builder).AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<AbilityEntity>? ordered = null;
    foreach (AbilitySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case AbilitySort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case AbilitySort.DisplayName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case AbilitySort.UniqueName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case AbilitySort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    AbilityEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<AbilityModel> abilities = await MapAsync(entities, cancellationToken);

    return new SearchResults<AbilityModel>(abilities, total);
  }

  private async Task<AbilityModel> MapAsync(AbilityEntity ability, CancellationToken cancellationToken)
  {
    return (await MapAsync([ability], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<AbilityModel>> MapAsync(IEnumerable<AbilityEntity> abilities, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = abilities.SelectMany(ability => ability.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return abilities.Select(mapper.ToAbility).ToList().AsReadOnly();
  }
}
