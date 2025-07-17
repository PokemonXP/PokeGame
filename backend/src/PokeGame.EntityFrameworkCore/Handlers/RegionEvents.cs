using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Regions.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class RegionEvents : IEventHandler<RegionCreated>,
  IEventHandler<RegionDeleted>,
  IEventHandler<RegionUniqueNameChanged>,
  IEventHandler<RegionUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<RegionCreated>, RegionEvents>();
    services.AddScoped<IEventHandler<RegionDeleted>, RegionEvents>();
    services.AddScoped<IEventHandler<RegionUniqueNameChanged>, RegionEvents>();
    services.AddScoped<IEventHandler<RegionUpdated>, RegionEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<RegionEvents> _logger;

  public RegionEvents(PokemonContext context, ILogger<RegionEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(RegionCreated @event, CancellationToken cancellationToken)
  {
    RegionEntity? region = await _context.Regions.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (region is not null)
    {
      _logger.LogUnexpectedVersion(@event, region);
      return;
    }

    region = new RegionEntity(@event);
    _context.Regions.Add(region);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(RegionDeleted @event, CancellationToken cancellationToken)
  {
    RegionEntity? region = await _context.Regions
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (region is null)
    {
      _logger.LogUnexpectedVersion(@event, region);
      return;
    }

    _context.Regions.Remove(region);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(RegionUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    RegionEntity? region = await _context.Regions
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (region is null || (region.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, region);
      return;
    }

    region.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(RegionUpdated @event, CancellationToken cancellationToken)
  {
    RegionEntity? region = await _context.Regions
  .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (region is null || (region.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, region);
      return;
    }

    region.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
