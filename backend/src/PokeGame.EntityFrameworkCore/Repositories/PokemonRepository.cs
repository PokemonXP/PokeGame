using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Pokemon;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class PokemonRepository : Repository, IPokemonRepository
{
  private readonly DbSet<PokemonEntity> _pokemon;

  public PokemonRepository(PokemonContext context, IEventStore eventStore) : base(eventStore)
  {
    _pokemon = context.Pokemon;
  }

  public async Task<Specimen?> LoadAsync(PokemonId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Specimen>(id.StreamId, cancellationToken);
  }

  public async Task<Specimen?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    PokemonId pokemonId;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      pokemonId = new(id);
      Specimen? pokemon = await LoadAsync(pokemonId, cancellationToken);
      if (pokemon is not null)
      {
        return pokemon;
      }
    }

    string uniqueNameNormalized = Helper.Normalize(idOrUniqueName);
    string? streamId = await _pokemon.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(streamId))
    {
      return null;
    }

    pokemonId = new(streamId);
    return await LoadAsync(pokemonId, cancellationToken);
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
