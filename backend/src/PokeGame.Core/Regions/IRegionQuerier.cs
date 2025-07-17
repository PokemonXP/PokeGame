using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Regions.Models;

namespace PokeGame.Core.Regions;

public interface IRegionQuerier
{
  Task<RegionId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<RegionModel> ReadAsync(Region region, CancellationToken cancellationToken = default);
  Task<RegionModel?> ReadAsync(RegionId id, CancellationToken cancellationToken = default);
  Task<RegionModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RegionModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<RegionModel>> SearchAsync(SearchRegionsPayload payload, CancellationToken cancellationToken = default);
}
