using Krakenar.Contracts;
using Krakenar.Core;
using PokeGame.Core.Regions.Models;

namespace PokeGame.Core.Regions.Queries;

internal record ReadRegion(Guid? Id, string? UniqueName) : IQuery<RegionModel?>;

/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadRegionHandler : IQueryHandler<ReadRegion, RegionModel?>
{
  private readonly IRegionQuerier _regionQuerier;

  public ReadRegionHandler(IRegionQuerier regionQuerier)
  {
    _regionQuerier = regionQuerier;
  }

  public async Task<RegionModel?> HandleAsync(ReadRegion query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, RegionModel> regions = new(capacity: 2);

    if (query.Id.HasValue)
    {
      RegionModel? region = await _regionQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (region is not null)
      {
        regions[region.Id] = region;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      RegionModel? region = await _regionQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (region is not null)
      {
        regions[region.Id] = region;
      }
    }

    if (regions.Count > 1)
    {
      throw TooManyResultsException<RegionModel>.ExpectedSingle(regions.Count);
    }

    return regions.SingleOrDefault().Value;
  }
}
