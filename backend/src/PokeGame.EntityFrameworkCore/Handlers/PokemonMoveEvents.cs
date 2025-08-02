using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Pokemon.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class PokemonMoveEvents : IEventHandler<PokemonMoveLearned>,
  IEventHandler<PokemonMoveRemembered>,
  IEventHandler<PokemonMoveSwapped>,
  IEventHandler<PokemonMoveUsed>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<PokemonMoveLearned>, PokemonMoveEvents>();
    services.AddScoped<IEventHandler<PokemonMoveRemembered>, PokemonMoveEvents>();
    services.AddScoped<IEventHandler<PokemonMoveSwapped>, PokemonMoveEvents>();
    services.AddScoped<IEventHandler<PokemonMoveUsed>, PokemonMoveEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<PokemonMoveEvents> _logger;

  public PokemonMoveEvents(PokemonContext context, ILogger<PokemonMoveEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(PokemonMoveLearned @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    MoveEntity move = await _context.Moves.SingleOrDefaultAsync(x => x.StreamId == @event.MoveId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The move entity 'StreamId={@event.MoveId}' was not found.");

    pokemon.LearnMove(move, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task HandleAsync(PokemonMoveRemembered @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    if (pokemon.RememberMove(@event))
    {
      await _context.SaveChangesAsync(cancellationToken);
    }
    else
    {
      _logger.LogError("The move (StreamId={MoveId}) was not found on Pokémon (StreamId={PokemonId}).", @event.MoveId, pokemon.StreamId);
    }
  }

  public async Task HandleAsync(PokemonMoveSwapped @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.SwapMoves(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task HandleAsync(PokemonMoveUsed @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.UseMove(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
