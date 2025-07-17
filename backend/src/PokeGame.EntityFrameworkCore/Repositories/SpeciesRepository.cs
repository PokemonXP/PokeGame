using Logitar.EventSourcing;
using PokeGame.Core.Species;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class SpeciesRepository : Repository, ISpeciesRepository
{
  public SpeciesRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<PokemonSpecies?> LoadAsync(SpeciesId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<PokemonSpecies>(id.StreamId, cancellationToken);
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
