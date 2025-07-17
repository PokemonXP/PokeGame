using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Regions.Commands;
using PokeGame.Core.Regions.Models;
using PokeGame.Core.Regions.Queries;

namespace PokeGame.Core.Regions;

public interface IRegionService
{
  Task<CreateOrReplaceRegionResult> CreateOrReplaceAsync(CreateOrReplaceRegionPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<RegionModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RegionModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<RegionModel>> SearchAsync(SearchRegionsPayload payload, CancellationToken cancellationToken = default);
  Task<RegionModel?> UpdateAsync(Guid id, UpdateRegionPayload payload, CancellationToken cancellationToken = default);
}

internal class RegionService : IRegionService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IRegionService, RegionService>()
      .AddTransient<IRegionManager, RegionManager>()
      .AddTransient<ICommandHandler<CreateOrReplaceRegion, CreateOrReplaceRegionResult>, CreateOrReplaceRegionHandler>()
      .AddTransient<ICommandHandler<DeleteRegion, RegionModel?>, DeleteRegionHandler>()
      .AddTransient<ICommandHandler<UpdateRegion, RegionModel?>, UpdateRegionHandler>()
      .AddTransient<IQueryHandler<ReadRegion, RegionModel?>, ReadRegionHandler>()
      .AddTransient<IQueryHandler<SearchRegions, SearchResults<RegionModel>>, SearchRegionsHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public RegionService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceRegionResult> CreateOrReplaceAsync(CreateOrReplaceRegionPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceRegion command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RegionModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteRegion command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RegionModel?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadRegion query = new(id, uniqueName);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<RegionModel>> SearchAsync(SearchRegionsPayload payload, CancellationToken cancellationToken)
  {
    SearchRegions query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<RegionModel?> UpdateAsync(Guid id, UpdateRegionPayload payload, CancellationToken cancellationToken)
  {
    UpdateRegion command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
