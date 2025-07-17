using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Varieties;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class VarietyRepository : Repository, IVarietyRepository
{
  private readonly DbSet<VarietyEntity> _varieties;

  public VarietyRepository(PokemonContext context, IEventStore eventStore) : base(eventStore)
  {
    _varieties = context.Varieties;
  }

  public async Task<Variety?> LoadAsync(VarietyId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Variety>(id.StreamId, cancellationToken);
  }
  public async Task<Variety?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    VarietyId varietyId;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      varietyId = new(id);
      Variety? variety = await LoadAsync(varietyId, cancellationToken);
      if (variety is not null)
      {
        return variety;
      }
    }

    string uniqueNameNormalized = Helper.Normalize(idOrUniqueName);
    string? streamId = await _varieties.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(streamId))
    {
      return null;
    }

    varietyId = new(streamId);
    return await LoadAsync(varietyId, cancellationToken);
  }

  public async Task SaveAsync(Variety variety, CancellationToken cancellationToken)
  {
    await base.SaveAsync(variety, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Variety> varieties, CancellationToken cancellationToken)
  {
    await base.SaveAsync(varieties, cancellationToken);
  }
}
