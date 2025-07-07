using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Pokemons.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class PokemonEvents : IEventHandler<PokemonCreated>,
  IEventHandler<PokemonNicknamed>,
  IEventHandler<PokemonUniqueNameChanged>,
  IEventHandler<PokemonUpdated>
{
  private readonly PokemonContext _context;
  private readonly ILogger<PokemonEvents> _logger;

  public PokemonEvents(PokemonContext context, ILogger<PokemonEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(PokemonCreated @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null)
    {
      FormEntity form = await _context.Forms
        .Include(x => x.Variety).ThenInclude(x => x!.Species)
        .SingleOrDefaultAsync(x => x.StreamId == @event.FormId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The form entity 'StreamId={@event.FormId}' was not found.");

      pokemon = new PokemonEntity(form, @event);

      _context.Pokemon.Add(pokemon);

      await _context.SaveChangesAsync(cancellationToken);
    }
    else
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
    }
  }

  public async Task HandleAsync(PokemonNicknamed @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.SetNickname(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task HandleAsync(PokemonUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task HandleAsync(PokemonUpdated @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
