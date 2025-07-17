namespace PokeGame.Core.Species;

public interface ISpeciesRepository
{
  Task<PokemonSpecies?> LoadAsync(SpeciesId speciesId, CancellationToken cancellationToken = default);

  Task SaveAsync(PokemonSpecies species, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<PokemonSpecies> species, CancellationToken cancellationToken = default);
}
