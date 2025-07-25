using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Inventory.Commands;
using PokeGame.Core.Inventory.Models;
using PokeGame.Core.Inventory.Queries;

namespace PokeGame.Core.Inventory;

public interface IInventoryService
{
  Task<InventoryItemModel> AddAsync(Guid trainerId, Guid itemId, InventoryQuantityPayload? payload = null, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<InventoryItemModel>> ReadAsync(Guid trainerId, CancellationToken cancellationToken = default);
  Task<InventoryItemModel?> ReadAsync(Guid trainerId, Guid itemId, CancellationToken cancellationToken = default);
  Task<InventoryItemModel> RemoveAsync(Guid trainerId, Guid itemId, InventoryQuantityPayload? payload = null, CancellationToken cancellationToken = default);
  Task<InventoryItemModel> UpdateAsync(Guid trainerId, Guid itemId, InventoryQuantityPayload payload, CancellationToken cancellationToken = default);
}

internal class InventoryService : IInventoryService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IInventoryService, InventoryService>();
    services.AddTransient<ICommandHandler<AddInventoryItem, InventoryItemModel>, AddInventoryItemHandler>();
    services.AddTransient<ICommandHandler<RemoveInventoryItem, InventoryItemModel>, RemoveInventoryItemHandler>();
    services.AddTransient<ICommandHandler<UpdateInventoryItem, InventoryItemModel>, UpdateInventoryItemHandler>();
    services.AddTransient<IQueryHandler<ReadInventoryItemQuery, InventoryItemModel?>, ReadInventoryItemQueryHandler>();
    services.AddTransient<IQueryHandler<ReadInventoryQuery, IReadOnlyCollection<InventoryItemModel>>, ReadInventoryQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public InventoryService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<InventoryItemModel> AddAsync(Guid trainerId, Guid itemId, InventoryQuantityPayload? payload, CancellationToken cancellationToken)
  {
    AddInventoryItem command = new(trainerId, itemId, payload ?? new());
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IReadOnlyCollection<InventoryItemModel>> ReadAsync(Guid trainerId, CancellationToken cancellationToken)
  {
    ReadInventoryQuery query = new(trainerId);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<InventoryItemModel?> ReadAsync(Guid trainerId, Guid itemId, CancellationToken cancellationToken)
  {
    ReadInventoryItemQuery query = new(trainerId, itemId);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<InventoryItemModel> RemoveAsync(Guid trainerId, Guid itemId, InventoryQuantityPayload? payload, CancellationToken cancellationToken)
  {
    RemoveInventoryItem command = new(trainerId, itemId, payload ?? new());
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<InventoryItemModel> UpdateAsync(Guid trainerId, Guid itemId, InventoryQuantityPayload payload, CancellationToken cancellationToken)
  {
    UpdateInventoryItem command = new(trainerId, itemId, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
