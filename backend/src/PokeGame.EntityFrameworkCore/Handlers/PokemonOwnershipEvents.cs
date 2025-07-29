using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Pokemon.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class PokemonOwnershipEvents : IEventHandler<PokemonCaught>,
  IEventHandler<PokemonDeposited>,
  IEventHandler<PokemonMoved>,
  IEventHandler<PokemonReceived>,
  IEventHandler<PokemonReleased>,
  IEventHandler<PokemonSwapped>,
  IEventHandler<PokemonWithdrew>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<PokemonCaught>, PokemonOwnershipEvents>();
    services.AddScoped<IEventHandler<PokemonDeposited>, PokemonOwnershipEvents>();
    services.AddScoped<IEventHandler<PokemonMoved>, PokemonOwnershipEvents>();
    services.AddScoped<IEventHandler<PokemonReceived>, PokemonOwnershipEvents>();
    services.AddScoped<IEventHandler<PokemonReleased>, PokemonOwnershipEvents>();
    services.AddScoped<IEventHandler<PokemonSwapped>, PokemonOwnershipEvents>();
    services.AddScoped<IEventHandler<PokemonWithdrew>, PokemonOwnershipEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<PokemonEvents> _logger;

  public PokemonOwnershipEvents(PokemonContext context, ILogger<PokemonEvents> logger)
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

    await UpdatePartySizeAsync(trainer, cancellationToken);
  }

  public async Task HandleAsync(PokemonDeposited @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.Deposit(@event);

    await _context.SaveChangesAsync(cancellationToken);

    if (pokemon.CurrentTrainerId.HasValue)
    {
      await UpdatePartySizeAsync(pokemon.CurrentTrainerId.Value, cancellationToken);
    }
  }

  public async Task HandleAsync(PokemonMoved @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.Move(@event);

    await _context.SaveChangesAsync(cancellationToken);

    if (pokemon.CurrentTrainerId.HasValue)
    {
      await UpdatePartySizeAsync(pokemon.CurrentTrainerId.Value, cancellationToken);
    }
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

    if (pokemon.OriginalTrainerId.HasValue && pokemon.OriginalTrainerId.Value != trainer.TrainerId)
    {
      await UpdatePartySizeAsync(pokemon.OriginalTrainerId.Value, cancellationToken);
    }
    await UpdatePartySizeAsync(trainer, cancellationToken);
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

    if (pokemon.CurrentTrainerId.HasValue)
    {
      await UpdatePartySizeAsync(pokemon.CurrentTrainerId.Value, cancellationToken);
    }
  }

  public async Task HandleAsync(PokemonSwapped @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.Swap(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task HandleAsync(PokemonWithdrew @event, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _context.Pokemon.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (pokemon is null || pokemon.Version != (@event.Version - 1))
    {
      _logger.LogUnexpectedVersion(@event, pokemon);
      return;
    }

    pokemon.Withdraw(@event);

    await _context.SaveChangesAsync(cancellationToken);

    if (pokemon.CurrentTrainerId.HasValue)
    {
      await UpdatePartySizeAsync(pokemon.CurrentTrainerId.Value, cancellationToken);
    }
  }

  private async Task UpdatePartySizeAsync(TrainerEntity trainer, CancellationToken cancellationToken)
  {
    await UpdatePartySizeAsync(trainer.TrainerId, cancellationToken);
  }
  private async Task UpdatePartySizeAsync(int trainerId, CancellationToken cancellationToken)
  {
    await _context.Trainers
      .Where(x => x.TrainerId == trainerId)
      .ExecuteUpdateAsync(updates => updates.SetProperty(
        x => x.PartySize,
        x => _context.Pokemon.Count(x => x.CurrentTrainerId == trainerId && x.Box == null)), cancellationToken);
  }
}
