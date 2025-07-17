namespace PokeGame.Core.Regions;

public interface IRegionRepository
{
  Task<Region?> LoadAsync(RegionId regionId, CancellationToken cancellationToken = default);

  Task SaveAsync(Region region, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Region> regions, CancellationToken cancellationToken = default);
}
