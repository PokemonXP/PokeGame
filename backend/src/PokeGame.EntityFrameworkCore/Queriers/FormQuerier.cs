using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class FormQuerier : IFormQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<FormEntity> _forms;
  private readonly ISqlHelper _sqlHelper;

  public FormQuerier(IActorService actorService, PokemonContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _forms = context.Forms;
    _sqlHelper = sqlHelper;
  }

  public async Task<FormModel> ReadAsync(FormId id, CancellationToken cancellationToken)
  {
    FormEntity form = await _forms.AsNoTracking()
      .Include(x => x.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The form entity 'StreamId={id}' was not found.");
    return await MapAsync(form, cancellationToken);
  }
  public async Task<FormModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FormEntity? form = await _forms.AsNoTracking()
      .Include(x => x.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return form is null ? null : await MapAsync(form, cancellationToken);
  }
  public async Task<FormModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    FormEntity? form = await _forms.AsNoTracking()
      .Include(x => x.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return form is null ? null : await MapAsync(form, cancellationToken);
  }

  public async Task<SearchResults<FormModel>> SearchAsync(Guid varietyId, SearchFormsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(PokemonDb.Forms.Table).SelectAll(PokemonDb.Forms.Table)
      .Where(PokemonDb.Forms.VarietyUid, Operators.IsEqualTo(varietyId))
      .ApplyIdFilter(PokemonDb.Forms.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, PokemonDb.Forms.UniqueName, PokemonDb.Forms.DisplayName);

    if (payload.Type.HasValue)
    {
      builder.WhereOr(
        new OperatorCondition(PokemonDb.Forms.PrimaryType, Operators.IsEqualTo(payload.Type.Value.ToString())),
        new OperatorCondition(PokemonDb.Forms.SecondaryType, Operators.IsEqualTo(payload.Type.Value.ToString())));
    }
    if (payload.AbilityId.HasValue)
    {
      builder.Join(PokemonDb.FormAbilities.FormId, PokemonDb.Forms.FormId)
        .Where(PokemonDb.FormAbilities.AbilityUid, Operators.IsEqualTo(payload.AbilityId.Value));
    }

    IQueryable<FormEntity> query = _forms.FromQuery(builder).AsNoTracking()
      .Include(x => x.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region);
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<FormEntity>? ordered = null;
    foreach (FormSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case FormSort.CreatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case FormSort.DisplayName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case FormSort.ExperienceYield:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.ExperienceYield) : query.OrderBy(x => x.ExperienceYield))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.ExperienceYield) : ordered.ThenBy(x => x.ExperienceYield));
          break;
        case FormSort.Height:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Height) : query.OrderBy(x => x.Height))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Height) : ordered.ThenBy(x => x.Height));
          break;
        case FormSort.UniqueName:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case FormSort.UpdatedOn:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
        case FormSort.Weight:
          ordered = ordered is null
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Weight) : query.OrderBy(x => x.Weight))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Weight) : ordered.ThenBy(x => x.Weight));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    FormEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<FormModel> forms = await MapAsync(entities, cancellationToken);

    return new SearchResults<FormModel>(forms, total);
  }

  private async Task<FormModel> MapAsync(FormEntity form, CancellationToken cancellationToken)
  {
    return (await MapAsync([form], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<FormModel>> MapAsync(IEnumerable<FormEntity> forms, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = forms.SelectMany(form => form.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return forms.Select(mapper.ToForm).ToList().AsReadOnly();
  }
}
