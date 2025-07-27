using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Pokemon.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class PokemonStorageEvents : IEventHandler<PokemonStored>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<PokemonStored>, PokemonStorageEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<PokemonStorageEvents> _logger;

  public PokemonStorageEvents(PokemonContext context, ILogger<PokemonStorageEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(PokemonStored @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null)
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.Store(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
