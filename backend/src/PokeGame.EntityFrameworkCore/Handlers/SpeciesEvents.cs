using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Species.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class SpeciesEvents : IEventHandler<SpeciesCreated>,
  IEventHandler<SpeciesDeleted>,
  IEventHandler<SpeciesRegionalNumberChanged>,
  IEventHandler<SpeciesUniqueNameChanged>,
  IEventHandler<SpeciesUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<SpeciesCreated>, SpeciesEvents>();
    services.AddScoped<IEventHandler<SpeciesDeleted>, SpeciesEvents>();
    services.AddScoped<IEventHandler<SpeciesUniqueNameChanged>, SpeciesEvents>();
    services.AddScoped<IEventHandler<SpeciesUpdated>, SpeciesEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<SpeciesEvents> _logger;

  public SpeciesEvents(PokemonContext context, ILogger<SpeciesEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(SpeciesCreated @event, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _context.Species.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (species is not null)
    {
      _logger.LogUnexpectedVersion(@event, species);
      return;
    }

    species = new SpeciesEntity(@event);
    _context.Species.Add(species);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(SpeciesDeleted @event, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _context.Species
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (species is null)
    {
      _logger.LogUnexpectedVersion(@event, species);
      return;
    }

    _context.Species.Remove(species);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(SpeciesRegionalNumberChanged @event, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _context.Species
      .Include(x => x.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (species is null || (species.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, species);
      return;
    }

    RegionEntity? region = null;
    if (@event.Number is not null)
    {
      region = await _context.Regions.SingleOrDefaultAsync(x => x.StreamId == @event.RegionId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The region entity 'StreamId={@event.RegionId}' was not found.");
    }

    RegionalNumberEntity? regionalNumber = species.SetRegionalNumber(region, @event);
    if (regionalNumber is not null && @event.Number is null)
    {
      _context.RegionalNumbers.Remove(regionalNumber);
    }

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(SpeciesUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _context.Species
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (species is null || (species.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, species);
      return;
    }

    species.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(SpeciesUpdated @event, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _context.Species
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (species is null || (species.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, species);
      return;
    }

    species.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
