namespace PokeGame.Core.Pokemon;

public interface IPokemonRepository
{
  Task<Pokemon2?> LoadAsync(PokemonId id, CancellationToken cancellationToken = default);

  Task SaveAsync(Pokemon2 pokemon, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Pokemon2> pokemon, CancellationToken cancellationToken = default);
}
