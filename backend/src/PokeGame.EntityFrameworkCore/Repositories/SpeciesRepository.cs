using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Species;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class SpeciesRepository : Repository, ISpeciesRepository
{
  private readonly DbSet<SpeciesEntity> _species;

  public SpeciesRepository(PokemonContext context, IEventStore eventStore) : base(eventStore)
  {
    _species = context.Species;
  }

  public async Task<PokemonSpecies?> LoadAsync(SpeciesId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<PokemonSpecies>(id.StreamId, cancellationToken);
  }

  public async Task<PokemonSpecies?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    SpeciesId speciesId;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      speciesId = new(id);
      PokemonSpecies? variety = await LoadAsync(speciesId, cancellationToken);
      if (variety is not null)
      {
        return variety;
      }
    }

    string uniqueNameNormalized = Helper.Normalize(idOrUniqueName);
    string? streamId = await _species.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(streamId))
    {
      return null;
    }

    speciesId = new(streamId);
    return await LoadAsync(speciesId, cancellationToken);
  }

  public async Task SaveAsync(PokemonSpecies species, CancellationToken cancellationToken)
  {
    await base.SaveAsync(species, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<PokemonSpecies> species, CancellationToken cancellationToken)
  {
    await base.SaveAsync(species, cancellationToken);
  }
}
