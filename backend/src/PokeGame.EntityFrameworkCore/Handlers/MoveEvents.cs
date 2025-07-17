using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Moves.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class MoveEvents : IEventHandler<MoveCreated>,
  IEventHandler<MoveDeleted>,
  IEventHandler<MoveUniqueNameChanged>,
  IEventHandler<MoveUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<MoveCreated>, MoveEvents>();
    services.AddScoped<IEventHandler<MoveDeleted>, MoveEvents>();
    services.AddScoped<IEventHandler<MoveUniqueNameChanged>, MoveEvents>();
    services.AddScoped<IEventHandler<MoveUpdated>, MoveEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<MoveEvents> _logger;

  public MoveEvents(PokemonContext context, ILogger<MoveEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(MoveCreated @event, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _context.Moves.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (move is not null)
    {
      _logger.LogUnexpectedVersion(@event, move);
      return;
    }

    move = new MoveEntity(@event);
    _context.Moves.Add(move);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(MoveDeleted @event, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _context.Moves
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (move is null)
    {
      _logger.LogUnexpectedVersion(@event, move);
      return;
    }

    _context.Moves.Remove(move);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(MoveUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _context.Moves
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (move is null || (move.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, move);
      return;
    }

    move.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(MoveUpdated @event, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _context.Moves
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (move is null || (move.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, move);
      return;
    }

    move.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
