using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Varieties.Commands;
using PokeGame.Core.Varieties.Models;
using PokeGame.Core.Varieties.Queries;

namespace PokeGame.Core.Varieties;

public interface IVarietyService
{
  Task<CreateOrReplaceVarietyResult> CreateOrReplaceAsync(CreateOrReplaceVarietyPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<VarietyModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<VarietyModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<VarietyModel>> SearchAsync(SearchVarietiesPayload payload, CancellationToken cancellationToken = default);
  Task<VarietyModel?> UpdateAsync(Guid id, UpdateVarietyPayload payload, CancellationToken cancellationToken = default);
}

internal class VarietyService : IVarietyService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IVarietyService, VarietyService>();
    services.AddTransient<IVarietyManager, VarietyManager>();
    services.AddTransient<ICommandHandler<CreateOrReplaceVariety, CreateOrReplaceVarietyResult>, CreateOrReplaceVarietyHandler>();
    services.AddTransient<ICommandHandler<DeleteVariety, VarietyModel?>, DeleteVarietyHandler>();
    services.AddTransient<ICommandHandler<UpdateVariety, VarietyModel?>, UpdateVarietyHandler>();
    services.AddTransient<IQueryHandler<ReadVariety, VarietyModel?>, ReadVarietyHandler>();
    services.AddTransient<IQueryHandler<SearchVarieties, SearchResults<VarietyModel>>, SearchVarietiesHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public VarietyService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceVarietyResult> CreateOrReplaceAsync(CreateOrReplaceVarietyPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceVariety command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<VarietyModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteVariety command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<VarietyModel?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadVariety query = new(id, uniqueName);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<VarietyModel>> SearchAsync(SearchVarietiesPayload payload, CancellationToken cancellationToken)
  {
    SearchVarieties query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<VarietyModel?> UpdateAsync(Guid id, UpdateVarietyPayload payload, CancellationToken cancellationToken)
  {
    UpdateVariety command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
