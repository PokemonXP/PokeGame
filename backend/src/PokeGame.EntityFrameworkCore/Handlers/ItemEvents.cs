using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Items.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class ItemEvents : IEventHandler<BattleItemPropertiesChanged>,
  IEventHandler<BerryPropertiesChanged>,
  IEventHandler<ItemCreated>,
  IEventHandler<ItemDeleted>,
  IEventHandler<ItemUniqueNameChanged>,
  IEventHandler<ItemUpdated>,
  IEventHandler<KeyItemPropertiesChanged>,
  IEventHandler<MaterialPropertiesChanged>,
  IEventHandler<MedicinePropertiesChanged>,
  IEventHandler<OtherItemPropertiesChanged>,
  IEventHandler<PokeBallPropertiesChanged>,
  IEventHandler<TechnicalMachinePropertiesChanged>,
  IEventHandler<TreasurePropertiesChanged>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<BattleItemPropertiesChanged>, ItemEvents>();
    services.AddScoped<IEventHandler<BerryPropertiesChanged>, ItemEvents>();
    services.AddScoped<IEventHandler<ItemCreated>, ItemEvents>();
    services.AddScoped<IEventHandler<ItemDeleted>, ItemEvents>();
    services.AddScoped<IEventHandler<ItemUniqueNameChanged>, ItemEvents>();
    services.AddScoped<IEventHandler<ItemUpdated>, ItemEvents>();
    services.AddScoped<IEventHandler<KeyItemPropertiesChanged>, ItemEvents>();
    services.AddScoped<IEventHandler<MaterialPropertiesChanged>, ItemEvents>();
    services.AddScoped<IEventHandler<MedicinePropertiesChanged>, ItemEvents>();
    services.AddScoped<IEventHandler<OtherItemPropertiesChanged>, ItemEvents>();
    services.AddScoped<IEventHandler<PokeBallPropertiesChanged>, ItemEvents>();
    services.AddScoped<IEventHandler<TechnicalMachinePropertiesChanged>, ItemEvents>();
    services.AddScoped<IEventHandler<TreasurePropertiesChanged>, ItemEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<ItemEvents> _logger;

  public ItemEvents(PokemonContext context, ILogger<ItemEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(BattleItemPropertiesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(BerryPropertiesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(ItemCreated @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is not null)
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item = new ItemEntity(@event);
    _context.Items.Add(item);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(ItemDeleted @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null)
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    _context.Items.Remove(item);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(ItemUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(ItemUpdated @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(KeyItemPropertiesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(MaterialPropertiesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(MedicinePropertiesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(OtherItemPropertiesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(PokeBallPropertiesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(TechnicalMachinePropertiesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    MoveEntity move = await _context.Moves.SingleOrDefaultAsync(x => x.StreamId == @event.Properties.MoveId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The move entity 'StreamId={@event.Properties.MoveId}' was not found.");

    item.SetProperties(move, @event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(TreasurePropertiesChanged @event, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _context.Items
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (item is null || (item.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, item);
      return;
    }

    item.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
