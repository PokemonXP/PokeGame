using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Forms.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class FormEvents : IEventHandler<FormCreated>,
  IEventHandler<FormDeleted>,
  IEventHandler<FormUniqueNameChanged>,
  IEventHandler<FormUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<FormCreated>, FormEvents>();
    services.AddScoped<IEventHandler<FormDeleted>, FormEvents>();
    services.AddScoped<IEventHandler<FormUniqueNameChanged>, FormEvents>();
    services.AddScoped<IEventHandler<FormUpdated>, FormEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<FormEvents> _logger;

  public FormEvents(PokemonContext context, ILogger<FormEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(FormCreated @event, CancellationToken cancellationToken)
  {
    FormEntity? form = await _context.Forms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (form is not null)
    {
      _logger.LogUnexpectedVersion(@event, form);
      return;
    }

    VarietyEntity variety = await _context.Varieties.Include(x => x.Species)
      .SingleOrDefaultAsync(x => x.StreamId == @event.VarietyId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The variety entity 'StreamId={@event.VarietyId}' was not found");

    form = new FormEntity(variety, @event);
    _context.Forms.Add(form);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(FormDeleted @event, CancellationToken cancellationToken)
  {
    FormEntity? form = await _context.Forms
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (form is null)
    {
      _logger.LogUnexpectedVersion(@event, form);
      return;
    }

    _context.Forms.Remove(form);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(FormUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    FormEntity? form = await _context.Forms
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (form is null || (form.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, form);
      return;
    }

    form.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(FormUpdated @event, CancellationToken cancellationToken)
  {
    FormEntity? form = await _context.Forms
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (form is null || (form.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, form);
      return;
    }

    form.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
