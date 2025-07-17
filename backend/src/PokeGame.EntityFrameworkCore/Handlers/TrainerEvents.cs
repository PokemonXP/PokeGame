using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Trainers.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class TrainerEvents : IEventHandler<TrainerCreated>,
  IEventHandler<TrainerDeleted>,
  IEventHandler<TrainerUniqueNameChanged>,
  IEventHandler<TrainerUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<TrainerCreated>, TrainerEvents>();
    services.AddScoped<IEventHandler<TrainerDeleted>, TrainerEvents>();
    services.AddScoped<IEventHandler<TrainerUniqueNameChanged>, TrainerEvents>();
    services.AddScoped<IEventHandler<TrainerUpdated>, TrainerEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<TrainerEvents> _logger;

  public TrainerEvents(PokemonContext context, ILogger<TrainerEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(TrainerCreated @event, CancellationToken cancellationToken)
  {
    TrainerEntity? trainer = await _context.Trainers.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (trainer is not null)
    {
      _logger.LogUnexpectedVersion(@event, trainer);
      return;
    }

    trainer = new TrainerEntity(@event);
    _context.Trainers.Add(trainer);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(TrainerDeleted @event, CancellationToken cancellationToken)
  {
    TrainerEntity? trainer = await _context.Trainers
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (trainer is null)
    {
      _logger.LogUnexpectedVersion(@event, trainer);
      return;
    }

    _context.Trainers.Remove(trainer);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(TrainerUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    TrainerEntity? trainer = await _context.Trainers
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (trainer is null || (trainer.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, trainer);
      return;
    }

    trainer.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(TrainerUpdated @event, CancellationToken cancellationToken)
  {
    TrainerEntity? trainer = await _context.Trainers
  .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (trainer is null || (trainer.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, trainer);
      return;
    }

    trainer.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
