using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Trainers;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class PokemonQuerier : IPokemonQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<FormEntity> _forms;
  private readonly DbSet<PokemonEntity> _pokemon;
  private readonly ISqlHelper _sqlHelper;

  public PokemonQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _forms = context.Forms;
    _pokemon = context.Pokemon;
    _sqlHelper = sqlHelper;
  }

  public async Task<PokemonId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _pokemon.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new PokemonId(streamId);
  }

  public async Task<Storage> GetStorageAsync(TrainerId trainerId, CancellationToken cancellationToken)
  {
    Guid trainerUid = trainerId.ToGuid();
    var slots = await _pokemon.AsNoTracking()
      .Where(x => x.CurrentTrainerUid == trainerUid)
      .Select(x => new { x.StreamId, x.Position, x.Box })
      .ToArrayAsync(cancellationToken);
    return new Storage(slots
      .Where(slot => !string.IsNullOrWhiteSpace(slot.StreamId) && slot.Position.HasValue)
      .Select(slot => new KeyValuePair<PokemonId, PokemonSlot>(
        new PokemonId(slot.StreamId),
        new PokemonSlot(new Position(slot.Position!.Value), slot.Box.HasValue ? new Box(slot.Box.Value) : null))));
  }

  public async Task<PokemonModel> ReadAsync(Specimen pokemon, CancellationToken cancellationToken)
  {
    return await ReadAsync(pokemon.Id, cancellationToken) ?? throw new InvalidOperationException($"The Pokémon entity 'StreamId={pokemon.Id}' was not found.");
  }
  public async Task<PokemonModel?> ReadAsync(PokemonId id, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _pokemon.AsNoTracking()
      .Include(x => x.CurrentTrainer)
      .Include(x => x.HeldItem)
      .Include(x => x.Moves).ThenInclude(x => x.Item)
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .Include(x => x.OriginalTrainer)
      .Include(x => x.PokeBall)
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    await FillAsync(pokemon, cancellationToken);

    return await MapAsync(pokemon, cancellationToken);
  }
  public async Task<PokemonModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _pokemon.AsNoTracking()
      .Include(x => x.CurrentTrainer)
      .Include(x => x.HeldItem)
      .Include(x => x.Moves).ThenInclude(x => x.Item)
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .Include(x => x.OriginalTrainer)
      .Include(x => x.PokeBall)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    await FillAsync(pokemon, cancellationToken);

    return await MapAsync(pokemon, cancellationToken);
  }
  public async Task<PokemonModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    PokemonEntity? pokemon = await _pokemon.AsNoTracking()
      .Include(x => x.CurrentTrainer)
      .Include(x => x.HeldItem)
      .Include(x => x.Moves).ThenInclude(x => x.Item)
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .Include(x => x.OriginalTrainer)
      .Include(x => x.PokeBall)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    await FillAsync(pokemon, cancellationToken);

    return await MapAsync(pokemon, cancellationToken);
  }

  public async Task<SearchResults<PokemonModel>> SearchAsync(SearchPokemonPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Pokemon.Table).SelectAll(PokemonDb.Pokemon.Table)
      .ApplyIdFilter(PokemonDb.Pokemon.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Pokemon.UniqueName, PokemonDb.Pokemon.Nickname);

    IQueryable<PokemonEntity> query = _pokemon.FromQuery(builder).AsNoTracking()
      .Include(x => x.CurrentTrainer)
      .Include(x => x.HeldItem)
      .Include(x => x.Moves).ThenInclude(x => x.Item)
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .Include(x => x.OriginalTrainer)
      .Include(x => x.PokeBall);
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<PokemonEntity>? ordered = null;
    foreach (PokemonSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case PokemonSort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case PokemonSort.Nickname:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Nickname) : query.OrderBy(x => x.Nickname))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Nickname) : ordered.ThenBy(x => x.Nickname));
          break;
        case PokemonSort.UniqueName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case PokemonSort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    PokemonEntity[] entities = await query.ToArrayAsync(cancellationToken);
    await FillAsync(entities, cancellationToken);
    IReadOnlyCollection<PokemonModel> pokemon = await MapAsync(entities, cancellationToken);

    return new SearchResults<PokemonModel>(pokemon, total);
  }

  private async Task FillAsync(PokemonEntity pokemon, CancellationToken cancellationToken)
  {
    FormEntity form = await _forms.AsNoTracking()
      .Include(x => x.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Variety).ThenInclude(x => x!.Moves).ThenInclude(x => x.Move)
      .Include(x => x.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.FormId == pokemon.FormId, cancellationToken)
      ?? throw new InvalidOperationException($"The Pokémon form entity 'FormId={pokemon.FormId}' was not found.");
    pokemon.Form = form;
  }
  private async Task FillAsync(IEnumerable<PokemonEntity> pokemonList, CancellationToken cancellationToken)
  {
    IEnumerable<int> formIds = pokemonList.Select(x => x.FormId).Distinct();
    Dictionary<int, FormEntity> forms = await _forms.AsNoTracking()
      .Include(x => x.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Variety).ThenInclude(x => x!.Moves).ThenInclude(x => x.Move)
      .Include(x => x.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .Where(x => formIds.Contains(x.FormId))
      .ToDictionaryAsync(x => x.FormId, x => x, cancellationToken);
    foreach (PokemonEntity pokemon in pokemonList)
    {
      if (forms.TryGetValue(pokemon.FormId, out FormEntity? form))
      {
        pokemon.Form = form;
      }
    }
  }

  private async Task<PokemonModel> MapAsync(PokemonEntity pokemon, CancellationToken cancellationToken)
  {
    return (await MapAsync([pokemon], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<PokemonModel>> MapAsync(IEnumerable<PokemonEntity> pokemon, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = pokemon.SelectMany(p => p.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return pokemon.Select(mapper.ToPokemon).ToList().AsReadOnly();
  }
}
