namespace PokeGame.Core.Pokemons;

public interface IPokemonRepository
{
  Task<Pokemon?> LoadAsync(PokemonId id, CancellationToken cancellationToken = default);

  Task SaveAsync(Pokemon pokemon, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Pokemon> pokemon, CancellationToken cancellationToken = default);
}
