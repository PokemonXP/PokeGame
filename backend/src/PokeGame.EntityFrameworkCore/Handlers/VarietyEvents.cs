using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Varieties.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class VarietyEvents : IEventHandler<VarietyCreated>,
  IEventHandler<VarietyDeleted>,
  IEventHandler<VarietyEvolutionMoveChanged>,
  IEventHandler<VarietyLevelMoveChanged>,
  IEventHandler<VarietyMoveRemoved>,
  IEventHandler<VarietyUniqueNameChanged>,
  IEventHandler<VarietyUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<VarietyCreated>, VarietyEvents>();
    services.AddScoped<IEventHandler<VarietyDeleted>, VarietyEvents>();
    services.AddScoped<IEventHandler<VarietyEvolutionMoveChanged>, VarietyEvents>();
    services.AddScoped<IEventHandler<VarietyLevelMoveChanged>, VarietyEvents>();
    services.AddScoped<IEventHandler<VarietyMoveRemoved>, VarietyEvents>();
    services.AddScoped<IEventHandler<VarietyUniqueNameChanged>, VarietyEvents>();
    services.AddScoped<IEventHandler<VarietyUpdated>, VarietyEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<VarietyEvents> _logger;

  public VarietyEvents(PokemonContext context, ILogger<VarietyEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(VarietyCreated @event, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _context.Varieties.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (variety is not null)
    {
      _logger.LogUnexpectedVersion(@event, variety);
      return;
    }

    SpeciesEntity species = await _context.Species
      .SingleOrDefaultAsync(x => x.StreamId == @event.SpeciesId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The species entity 'StreamId={@event.SpeciesId}' was not found.");

    variety = new VarietyEntity(species, @event);
    _context.Varieties.Add(variety);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(VarietyDeleted @event, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _context.Varieties
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (variety is null)
    {
      _logger.LogUnexpectedVersion(@event, variety);
      return;
    }

    _context.Varieties.Remove(variety);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(VarietyEvolutionMoveChanged @event, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _context.Varieties
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (variety is null || (variety.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, variety);
      return;
    }

    MoveEntity move = await _context.Moves.SingleOrDefaultAsync(x => x.StreamId == @event.MoveId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The move entity 'StreamId={@event.MoveId}' was not found.");

    variety.SetEvolutionMove(move, @event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(VarietyLevelMoveChanged @event, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _context.Varieties
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (variety is null || (variety.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, variety);
      return;
    }

    MoveEntity move = await _context.Moves.SingleOrDefaultAsync(x => x.StreamId == @event.MoveId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The move entity 'StreamId={@event.MoveId}' was not found.");

    variety.SetLevelMove(move, @event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(VarietyMoveRemoved @event, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _context.Varieties
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (variety is null || (variety.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, variety);
      return;
    }

    VarietyMoveEntity? varietyMove = variety.RemoveMove(@event);
    if (varietyMove is not null)
    {
      _context.VarietyMoves.Remove(varietyMove);
    }

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(VarietyUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _context.Varieties
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (variety is null || (variety.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, variety);
      return;
    }

    variety.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(VarietyUpdated @event, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _context.Varieties
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (variety is null || (variety.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, variety);
      return;
    }

    variety.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
