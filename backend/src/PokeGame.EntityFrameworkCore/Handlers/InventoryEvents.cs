using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Inventory;
using PokeGame.Core.Inventory.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class InventoryEvents : IEventHandler<InventoryItemAdded>, IEventHandler<InventoryItemRemoved>, IEventHandler<InventoryItemUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<InventoryItemAdded>, InventoryEvents>();
    services.AddScoped<IEventHandler<InventoryItemRemoved>, InventoryEvents>();
    services.AddScoped<IEventHandler<InventoryItemUpdated>, InventoryEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<InventoryEvents> _logger;

  public InventoryEvents(PokemonContext context, ILogger<InventoryEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(InventoryItemAdded @event, CancellationToken cancellationToken)
  {
    Guid trainerUid = new TrainerInventoryId(@event.StreamId).TrainerId.ToGuid();
    Guid itemUid = @event.ItemId.ToGuid();
    InventoryEntity? inventory = await _context.Inventory
      .SingleOrDefaultAsync(x => x.TrainerUid == trainerUid && x.ItemUid == itemUid, cancellationToken);
    if (inventory is null)
    {
      TrainerEntity trainer = await _context.Trainers.SingleOrDefaultAsync(x => x.Id == trainerUid, cancellationToken)
        ?? throw new InvalidOperationException($"The trainer entity 'Id={trainerUid}' was not found.");
      ItemEntity item = await _context.Items.SingleOrDefaultAsync(x => x.Id == itemUid, cancellationToken)
        ?? throw new InvalidOperationException($"The trainer entity 'Id={itemUid}' was not found.");

      inventory = new InventoryEntity(trainer, item, @event);
      _context.Inventory.Add(inventory);
    }
    else
    {
      inventory.Add(@event);
    }

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(InventoryItemRemoved @event, CancellationToken cancellationToken)
  {
    Guid trainerUid = new TrainerInventoryId(@event.StreamId).TrainerId.ToGuid();
    Guid itemUid = @event.ItemId.ToGuid();
    InventoryEntity? inventory = await _context.Inventory
      .SingleOrDefaultAsync(x => x.TrainerUid == trainerUid && x.ItemUid == itemUid, cancellationToken);
    if (inventory is null)
    {
      _logger.LogError("No inventory line was found for trainer 'Id={TrainerId}' and item 'Id={ItemId}'.", trainerUid, itemUid);
    }
    else
    {
      inventory.Remove(@event);
      if (inventory.Quantity <= 0)
      {
        _context.Inventory.Remove(inventory);
      }

      await _context.SaveChangesAsync(cancellationToken);
      _logger.LogSuccess(@event);
    }
  }

  public async Task HandleAsync(InventoryItemUpdated @event, CancellationToken cancellationToken)
  {
    Guid trainerUid = new TrainerInventoryId(@event.StreamId).TrainerId.ToGuid();
    Guid itemUid = @event.ItemId.ToGuid();
    InventoryEntity? inventory = await _context.Inventory
      .SingleOrDefaultAsync(x => x.TrainerUid == trainerUid && x.ItemUid == itemUid, cancellationToken);
    if (inventory is null)
    {
      if (@event.Quantity < 1)
      {
        _logger.LogWarning("No inventory line was found for trainer 'Id={TrainerId}' and item 'Id={ItemId}'.", trainerUid, itemUid);
        return;
      }

      TrainerEntity trainer = await _context.Trainers.SingleOrDefaultAsync(x => x.Id == trainerUid, cancellationToken)
        ?? throw new InvalidOperationException($"The trainer entity 'Id={trainerUid}' was not found.");
      ItemEntity item = await _context.Items.SingleOrDefaultAsync(x => x.Id == itemUid, cancellationToken)
        ?? throw new InvalidOperationException($"The trainer entity 'Id={itemUid}' was not found.");

      inventory = new InventoryEntity(trainer, item, @event);
      _context.Inventory.Add(inventory);
    }
    else
    {
      inventory.Update(@event);
      if (inventory.Quantity <= 0)
      {
        _context.Inventory.Remove(inventory);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
