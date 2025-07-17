using Krakenar.Core;
using PokeGame.Core.Regions.Models;

namespace PokeGame.Core.Regions.Commands;

internal record DeleteRegion(Guid Id) : ICommand<RegionModel?>;

internal class DeleteRegionHandler : ICommandHandler<DeleteRegion, RegionModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;

  public DeleteRegionHandler(IApplicationContext applicationContext, IRegionQuerier regionQuerier, IRegionRepository regionRepository)
  {
    _applicationContext = applicationContext;
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
  }

  public async Task<RegionModel?> HandleAsync(DeleteRegion command, CancellationToken cancellationToken)
  {
    RegionId regionId = new(command.Id);
    Region? region = await _regionRepository.LoadAsync(regionId, cancellationToken);
    if (region is null)
    {
      return null;
    }
    RegionModel model = await _regionQuerier.ReadAsync(region, cancellationToken);

    region.Delete(_applicationContext.ActorId);
    await _regionRepository.SaveAsync(region, cancellationToken);

    return model;
  }
}
