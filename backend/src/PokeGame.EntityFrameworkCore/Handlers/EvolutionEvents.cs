using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Evolutions.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class EvolutionEvents : IEventHandler<EvolutionCreated>, IEventHandler<EvolutionDeleted>, IEventHandler<EvolutionUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<EvolutionCreated>, EvolutionEvents>();
    services.AddScoped<IEventHandler<EvolutionDeleted>, EvolutionEvents>();
    services.AddScoped<IEventHandler<EvolutionUpdated>, EvolutionEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<EvolutionEvents> _logger;

  public EvolutionEvents(PokemonContext context, ILogger<EvolutionEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(EvolutionCreated @event, CancellationToken cancellationToken)
  {
    EvolutionEntity? evolution = await _context.Evolutions.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (evolution is not null)
    {
      _logger.LogUnexpectedVersion(@event, evolution);
      return;
    }

    FormEntity source = null!; // TODO(fpion): implement
    FormEntity target = null!; // TODO(fpion): implement
    ItemEntity? item = null; // TODO(fpion): implement

    evolution = new EvolutionEntity(source, target, item, @event);
    _context.Evolutions.Add(evolution);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(EvolutionDeleted @event, CancellationToken cancellationToken)
  {
    EvolutionEntity? evolution = await _context.Evolutions
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (evolution is null)
    {
      _logger.LogUnexpectedVersion(@event, evolution);
      return;
    }

    _context.Evolutions.Remove(evolution);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(EvolutionUpdated @event, CancellationToken cancellationToken)
  {
    EvolutionEntity? evolution = await _context.Evolutions
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (evolution is null || (evolution.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, evolution);
      return;
    }

    evolution.Update(@event);
    evolution.SetHeldItem(item: null); // TODO(fpion): implement
    evolution.SetKnownMove(move: null); // TODO(fpion): implement

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
