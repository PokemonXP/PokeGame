using Logitar.EventSourcing;
using PokeGame.Core.Regions;
using PokeGame.Core.Species.Events;

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
    bool hasNumberChanged = false;
    bool hasUniqueNameChanged = false;
    Dictionary<RegionId, Number> regionalNumbers = new(capacity: species.RegionalNumbers.Count);
    foreach (IEvent change in species.Changes)
    {
      if (change is SpeciesCreated)
      {
        hasNumberChanged = true;
        hasUniqueNameChanged = true;
      }
      else if (change is SpeciesUniqueNameChanged)
      {
        hasUniqueNameChanged = true;
      }
      else if (change is SpeciesRegionalNumberChanged changed && changed.Number is not null)
      {
        regionalNumbers[changed.RegionId] = changed.Number;
      }
    }

    if (hasNumberChanged)
    {
      SpeciesId? conflictId = await _speciesQuerier.FindIdAsync(species.Number, regionId: null, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(species.Id))
      {
        throw new NumberAlreadyUsedException(species, conflictId.Value);
      }
    }

    if (hasUniqueNameChanged)
    {
      SpeciesId? conflictId = await _speciesQuerier.FindIdAsync(species.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(species.Id))
      {
        throw new UniqueNameAlreadyUsedException(species, conflictId.Value);
      }
    }

    foreach (KeyValuePair<RegionId, Number> regionalNumber in regionalNumbers)
    {
      SpeciesId? conflictId = await _speciesQuerier.FindIdAsync(regionalNumber.Value, regionalNumber.Key, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(species.Id))
      {
        throw new NumberAlreadyUsedException(species, conflictId.Value, regionalNumber.Key);
      }
    }

    await _speciesRepository.SaveAsync(species, cancellationToken);
  }
}
