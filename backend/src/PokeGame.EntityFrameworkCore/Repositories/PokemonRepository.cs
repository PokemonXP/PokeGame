using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class PokemonRepository : Repository, IPokemonRepository
{
  public PokemonRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Pokemon2?> LoadAsync(PokemonId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Pokemon2>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Pokemon2 pokemon, CancellationToken cancellationToken)
  {
    await base.SaveAsync(pokemon, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Pokemon2> pokemon, CancellationToken cancellationToken)
  {
    await base.SaveAsync(pokemon, cancellationToken);
  }
}
