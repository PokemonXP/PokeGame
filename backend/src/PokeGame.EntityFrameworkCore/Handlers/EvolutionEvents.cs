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

    Guid[] formIds = [@event.SourceId.ToGuid(), @event.TargetId.ToGuid()];
    FormEntity[] forms = await _context.Forms.Where(x => formIds.Contains(x.Id)).ToArrayAsync(cancellationToken);
    FormEntity source = forms.SingleOrDefault(form => form.Id == @event.SourceId.ToGuid())
      ?? throw new InvalidOperationException($"The form entity 'StreamId={@event.SourceId}' was not found.");
    FormEntity target = forms.SingleOrDefault(form => form.Id == @event.TargetId.ToGuid())
      ?? throw new InvalidOperationException($"The form entity 'StreamId={@event.TargetId}' was not found.");

    ItemEntity? item = null;
    if (@event.ItemId.HasValue)
    {
      item = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == @event.ItemId.Value.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'StreamId={@event.ItemId}' was not found.");
    }

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

    if (@event.HeldItemId is not null)
    {
      ItemEntity? heldItem = null;
      if (@event.HeldItemId.Value.HasValue)
      {
        heldItem = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == @event.HeldItemId.Value.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The item entity 'StreamId={@event.HeldItemId.Value}' was not found.");
      }
      evolution.SetHeldItem(heldItem);
    }

    if (@event.KnownMoveId is not null)
    {
      MoveEntity? knownMove = null;
      if (@event.KnownMoveId.Value.HasValue)
      {
        knownMove = await _context.Moves.SingleOrDefaultAsync(x => x.StreamId == @event.KnownMoveId.Value.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The move entity 'StreamId={@event.KnownMoveId.Value}' was not found.");
      }
      evolution.SetKnownMove(knownMove);
    }

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
