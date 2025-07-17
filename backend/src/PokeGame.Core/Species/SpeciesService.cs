using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Species.Commands;
using PokeGame.Core.Species.Models;
using PokeGame.Core.Species.Queries;

namespace PokeGame.Core.Species;

public interface ISpeciesService
{
  Task<CreateOrReplaceSpeciesResult> CreateOrReplaceAsync(CreateOrReplaceSpeciesPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(Guid? id = null, int? number = null, string? uniqueName = null, Guid? regionId = null, CancellationToken cancellationToken = default);
  Task<SearchResults<SpeciesModel>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> UpdateAsync(Guid id, UpdateSpeciesPayload payload, CancellationToken cancellationToken = default);
}

internal class SpeciesService : ISpeciesService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ISpeciesService, SpeciesService>();
    services.AddTransient<ISpeciesManager, SpeciesManager>();
    services.AddTransient<ICommandHandler<CreateOrReplaceSpecies, CreateOrReplaceSpeciesResult>, CreateOrReplaceSpeciesHandler>();
    services.AddTransient<ICommandHandler<DeleteSpecies, SpeciesModel?>, DeleteSpeciesHandler>();
    services.AddTransient<ICommandHandler<UpdateSpecies, SpeciesModel?>, UpdateSpeciesHandler>();
    services.AddTransient<IQueryHandler<ReadSpecies, SpeciesModel?>, ReadSpeciesHandler>();
    services.AddTransient<IQueryHandler<SearchSpecies, SearchResults<SpeciesModel>>, SearchSpeciesHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public SpeciesService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceSpeciesResult> CreateOrReplaceAsync(CreateOrReplaceSpeciesPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpecies command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SpeciesModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteSpecies command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SpeciesModel?> ReadAsync(Guid? id, int? number, string? uniqueName, Guid? regionId, CancellationToken cancellationToken)
  {
    ReadSpecies query = new(id, number, uniqueName, regionId);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<SpeciesModel>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken)
  {
    SearchSpecies query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SpeciesModel?> UpdateAsync(Guid id, UpdateSpeciesPayload payload, CancellationToken cancellationToken)
  {
    UpdateSpecies command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
