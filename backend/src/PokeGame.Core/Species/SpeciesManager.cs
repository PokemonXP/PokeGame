using Logitar.EventSourcing;
using PokeGame.Core.Regions;
using PokeGame.Core.Species.Events;
using PokeGame.Core.Species.Models;

namespace PokeGame.Core.Species;

internal interface ISpeciesManager
{
  Task<IReadOnlyDictionary<RegionId, Number?>> FindRegionalNumbersAsync(IEnumerable<RegionalNumberPayload> payloads, string propertyName, CancellationToken cancellationToken = default);
  Task SaveAsync(PokemonSpecies species, CancellationToken cancellationToken = default);
}

internal class SpeciesManager : ISpeciesManager
{
  private readonly IRegionQuerier _regionQuerier;
  private readonly ISpeciesQuerier _speciesQuerier;
  private readonly ISpeciesRepository _speciesRepository;

  public SpeciesManager(IRegionQuerier regionQuerier, ISpeciesQuerier speciesQuerier, ISpeciesRepository speciesRepository)
  {
    _regionQuerier = regionQuerier;
    _speciesQuerier = speciesQuerier;
    _speciesRepository = speciesRepository;
  }

  public async Task<IReadOnlyDictionary<RegionId, Number?>> FindRegionalNumbersAsync(IEnumerable<RegionalNumberPayload> payloads, string propertyName, CancellationToken cancellationToken)
  {
    int capacity = payloads.Count();
    Dictionary<RegionId, Number?> regionalNumbers = new(capacity);

    if (capacity > 0)
    {
      IReadOnlyCollection<RegionKey> keys = await _regionQuerier.GetKeysAsync(cancellationToken);
      Dictionary<Guid, RegionId> ids = new(capacity: keys.Count);
      Dictionary<string, RegionId> uniqueNames = new(capacity: keys.Count);
      foreach (RegionKey key in keys)
      {
        ids[key.Id] = key.RegionId;
        uniqueNames[Normalize(key.UniqueName)] = key.RegionId;
      }

      List<string> missing = new(capacity);
      foreach (RegionalNumberPayload payload in payloads)
      {
        if (!string.IsNullOrWhiteSpace(payload.Region))
        {
          RegionId regionId = new();
          Number? number = payload.Number < 1 ? null : new(payload.Number);
          bool found = false;

          if (Guid.TryParse(payload.Region, out Guid id))
          {
            found = ids.TryGetValue(id, out regionId);
          }
          if (!found)
          {
            found = uniqueNames.TryGetValue(Normalize(payload.Region), out regionId);
          }

          if (found)
          {
            regionalNumbers[regionId] = number;
          }
          else
          {
            missing.Add(payload.Region);
          }
        }
      }

      if (missing.Count > 0)
      {
        throw new RegionsNotFoundException(missing, propertyName);
      }
    }

    return regionalNumbers;
  }
  private static string Normalize(string value) => value.Trim().ToLowerInvariant();

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
