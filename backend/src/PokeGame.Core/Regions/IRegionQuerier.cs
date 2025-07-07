using Krakenar.Contracts.Search;
using PokeGame.Core.Regions.Models;

namespace PokeGame.Core.Regions;

public interface IRegionQuerier
{
  Task<RegionModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RegionModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<RegionModel>> SearchAsync(SearchRegionsPayload payload, CancellationToken cancellationToken = default);
}
