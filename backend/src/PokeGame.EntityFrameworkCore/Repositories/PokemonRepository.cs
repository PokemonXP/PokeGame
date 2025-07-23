using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class PokemonRepository : Repository, IPokemonRepository
{
  public PokemonRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Specimen?> LoadAsync(PokemonId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Specimen>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Specimen>> LoadAsync(IEnumerable<PokemonId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync<Specimen>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Specimen pokemon, CancellationToken cancellationToken)
  {
    await base.SaveAsync(pokemon, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Specimen> specimens, CancellationToken cancellationToken)
  {
    await base.SaveAsync(specimens, cancellationToken);
  }
}
