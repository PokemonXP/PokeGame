namespace PokeGame.Core.Evolutions;

public interface IEvolutionRepository
{
  Task<Evolution?> LoadAsync(EvolutionId evolutionId, CancellationToken cancellationToken = default);

  Task SaveAsync(Evolution evolution, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Evolution> evolutions, CancellationToken cancellationToken = default);
}
