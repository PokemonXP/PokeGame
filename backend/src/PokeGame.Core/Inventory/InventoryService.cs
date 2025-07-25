using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Inventory.Commands;
using PokeGame.Core.Inventory.Models;

namespace PokeGame.Core.Inventory;

public interface IInventoryService
{
  Task<InventoryItemModel> AddAsync(Guid trainerId, Guid itemId, AddInventoryItemPayload? payload = null, CancellationToken cancellationToken = default);
}

internal class InventoryService : IInventoryService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IInventoryService, InventoryService>();
    services.AddTransient<ICommandHandler<AddInventoryItem, InventoryItemModel>, AddInventoryItemHandler>();
  }

  private readonly ICommandBus _commandBus;

  public InventoryService(ICommandBus commandBus)
  {
    _commandBus = commandBus;
  }

  public async Task<InventoryItemModel> AddAsync(Guid trainerId, Guid itemId, AddInventoryItemPayload? payload, CancellationToken cancellationToken)
  {
    AddInventoryItem command = new(trainerId, itemId, payload ?? new());
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
