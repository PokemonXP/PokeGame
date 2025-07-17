namespace PokeGame.Core.Species;

internal interface ISpeciesManager
{
  Task SaveAsync(PokemonSpecies species, CancellationToken cancellationToken = default);
}

internal class SpeciesManager : ISpeciesManager
{
  private readonly ISpeciesQuerier _speciesQuerier;
  private readonly ISpeciesRepository _speciesRepository;

  public SpeciesManager(ISpeciesQuerier speciesQuerier, ISpeciesRepository speciesRepository)
  {
    _speciesQuerier = speciesQuerier;
    _speciesRepository = speciesRepository;
  }

  public async Task SaveAsync(PokemonSpecies species, CancellationToken cancellationToken)
  {
    // TODO(fpion): UniqueName + Number + RegionalNumbers

    await _speciesRepository.SaveAsync(species, cancellationToken);
  }
}
