using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Pokemon.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class PokemonOwnershipEvents : IEventHandler<PokemonCaught>, IEventHandler<PokemonReceived>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<PokemonCaught>, PokemonOwnershipEvents>();
    services.AddScoped<IEventHandler<PokemonReceived>, PokemonOwnershipEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<PokemonOwnershipEvents> _logger;

  public PokemonOwnershipEvents(PokemonContext context, ILogger<PokemonOwnershipEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(PokemonCaught @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    TrainerEntity trainer = await _context.Trainers.SingleOrDefaultAsync(x => x.StreamId == @event.TrainerId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The trainer entity 'StreamId={@event.TrainerId}' was not found.");
    ItemEntity pokeBall = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == @event.PokeBallId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The item entity 'StreamId={@event.PokeBallId}' was not found.");

    pokemon.Catch(trainer, pokeBall, @event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(PokemonReceived @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    TrainerEntity trainer = await _context.Trainers.SingleOrDefaultAsync(x => x.StreamId == @event.TrainerId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The trainer entity 'StreamId={@event.TrainerId}' was not found.");
    ItemEntity pokeBall = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == @event.PokeBallId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The item entity 'StreamId={@event.PokeBallId}' was not found.");

    pokemon.Receive(trainer, pokeBall, @event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
