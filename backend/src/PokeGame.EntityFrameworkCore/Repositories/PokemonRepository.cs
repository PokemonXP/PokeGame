using Logitar.EventSourcing;
using PokeGame.Core.Pokemons;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class PokemonRepository : Repository, IPokemonRepository
{
  public PokemonRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Pokemon?> LoadAsync(PokemonId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Pokemon>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Pokemon pokemon, CancellationToken cancellationToken)
  {
    await base.SaveAsync(pokemon, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Pokemon> pokemon, CancellationToken cancellationToken)
  {
    await base.SaveAsync(pokemon, cancellationToken);
  }
}
