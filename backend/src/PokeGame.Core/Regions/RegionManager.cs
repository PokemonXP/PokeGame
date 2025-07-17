using PokeGame.Core.Regions.Events;

namespace PokeGame.Core.Regions;

internal interface IRegionManager
{
  Task SaveAsync(Region region, CancellationToken cancellationToken = default);
}

internal class RegionManager : IRegionManager
{
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;

  public RegionManager(IRegionQuerier regionQuerier, IRegionRepository regionRepository)
  {
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
  }

  public async Task SaveAsync(Region region, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = region.Changes.Any(change => change is RegionCreated || change is RegionUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      RegionId? conflictId = await _regionQuerier.FindIdAsync(region.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(region.Id))
      {
        throw new UniqueNameAlreadyUsedException(region, conflictId.Value);
      }
    }

    await _regionRepository.SaveAsync(region, cancellationToken);
  }
}
