using Krakenar.Core;
using PokeGame.Core.Regions.Commands;
using PokeGame.Core.Regions.Models;

namespace PokeGame.Core.Regions;

public interface IRegionService
{
  Task<CreateOrReplaceRegionResult> CreateOrReplaceAsync(CreateOrReplaceRegionPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
}

internal class RegionService : IRegionService
{
  private readonly ICommandBus _commandBus;

  public RegionService(ICommandBus commandBus)
  {
    _commandBus = commandBus;
  }

  public async Task<CreateOrReplaceRegionResult> CreateOrReplaceAsync(CreateOrReplaceRegionPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceRegionCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
