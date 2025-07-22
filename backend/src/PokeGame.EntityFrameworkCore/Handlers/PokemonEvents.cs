using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Pokemon.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class PokemonEvents : IEventHandler<PokemonCaught>,
  IEventHandler<PokemonCreated>,
  IEventHandler<PokemonDeleted>,
  IEventHandler<PokemonHealed>,
  IEventHandler<PokemonItemHeld>,
  IEventHandler<PokemonItemRemoved>,
  IEventHandler<PokemonMoveLearned>,
  IEventHandler<PokemonMoveRemembered>,
  IEventHandler<PokemonMoveSwitched>,
  IEventHandler<PokemonNicknamed>,
  IEventHandler<PokemonReceived>,
  IEventHandler<PokemonReleased>,
  IEventHandler<PokemonUniqueNameChanged>,
  IEventHandler<PokemonUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<PokemonCaught>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonCreated>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonDeleted>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonHealed>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonItemHeld>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonItemRemoved>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonMoveLearned>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonMoveRemembered>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonMoveSwitched>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonNicknamed>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonReceived>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonReleased>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonUniqueNameChanged>, PokemonEvents>();
    services.AddScoped<IEventHandler<PokemonUpdated>, PokemonEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<PokemonEvents> _logger;

  public PokemonEvents(PokemonContext context, ILogger<PokemonEvents> logger)
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
  }

  public async Task HandleAsync(PokemonCreated @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
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

  public async Task HandleAsync(PokemonDeleted @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null)
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
    }
    else
    {
      _context.Pokemon.Remove(pokemon);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(PokemonHealed @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon
      .Include(x => x.Moves)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.Heal(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }


  public async Task HandleAsync(PokemonItemHeld @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    ItemEntity item = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == @event.ItemId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The item entity 'StreamId={@event.ItemId}' was not found.");

    pokemon.HoldItem(item, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task HandleAsync(PokemonItemRemoved @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.RemoveItem(@event);

    await _context.SaveChangesAsync(cancellationToken);
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

  public async Task HandleAsync(PokemonMoveSwitched @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.SwitchMoves(@event);

    await _context.SaveChangesAsync(cancellationToken);
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
  }

  public async Task HandleAsync(PokemonReleased @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.Release(@event);

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
