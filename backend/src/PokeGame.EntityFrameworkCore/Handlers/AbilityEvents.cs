using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Abilities.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class AbilityEvents : IEventHandler<AbilityCreated>,
  IEventHandler<AbilityDeleted>,
  IEventHandler<AbilityUniqueNameChanged>,
  IEventHandler<AbilityUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<AbilityCreated>, AbilityEvents>();
    services.AddScoped<IEventHandler<AbilityDeleted>, AbilityEvents>();
    services.AddScoped<IEventHandler<AbilityUniqueNameChanged>, AbilityEvents>();
    services.AddScoped<IEventHandler<AbilityUpdated>, AbilityEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<AbilityEvents> _logger;

  public AbilityEvents(PokemonContext context, ILogger<AbilityEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(AbilityCreated @event, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _context.Abilities.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ability is not null)
    {
      _logger.LogUnexpectedVersion(@event, ability);
      return;
    }

    ability = new AbilityEntity(@event);
    _context.Abilities.Add(ability);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(AbilityDeleted @event, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _context.Abilities
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ability is null)
    {
      _logger.LogUnexpectedVersion(@event, ability);
      return;
    }

    _context.Abilities.Remove(ability);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(AbilityUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _context.Abilities
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ability is null || (ability.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, ability);
      return;
    }

    ability.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(AbilityUpdated @event, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _context.Abilities
  .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ability is null || (ability.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, ability);
      return;
    }

    ability.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
