using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Battles.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class BattleEvents : IEventHandler<BattleCancelled>,
  IEventHandler<BattleDeleted>,
  IEventHandler<BattleEscaped>,
  IEventHandler<BattleExperienceGained>,
  IEventHandler<BattleMoveUsed>,
  IEventHandler<BattlePokemonSwitched>,
  IEventHandler<BattleReset>,
  IEventHandler<BattleStarted>,
  IEventHandler<BattleUpdated>,
  IEventHandler<TrainerBattleCreated>,
  IEventHandler<WildPokemonBattleCreated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<BattleCancelled>, BattleEvents>();
    services.AddScoped<IEventHandler<BattleDeleted>, BattleEvents>();
    services.AddScoped<IEventHandler<BattleEscaped>, BattleEvents>();
    services.AddScoped<IEventHandler<BattleExperienceGained>, BattleEvents>();
    services.AddScoped<IEventHandler<BattleMoveUsed>, BattleEvents>();
    services.AddScoped<IEventHandler<BattlePokemonSwitched>, BattleEvents>();
    services.AddScoped<IEventHandler<BattleReset>, BattleEvents>();
    services.AddScoped<IEventHandler<BattleStarted>, BattleEvents>();
    services.AddScoped<IEventHandler<BattleUpdated>, BattleEvents>();
    services.AddScoped<IEventHandler<TrainerBattleCreated>, BattleEvents>();
    services.AddScoped<IEventHandler<WildPokemonBattleCreated>, BattleEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<BattleEvents> _logger;

  public BattleEvents(PokemonContext context, ILogger<BattleEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(BattleCancelled @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is null || (battle.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    battle.Cancel(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(BattleDeleted @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is null)
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    _context.Battles.Remove(battle);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(BattleEscaped @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is null || (battle.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    battle.Escape(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(BattlePokemonSwitched @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles
      .Include(x => x.Pokemon).ThenInclude(x => x.Pokemon)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is null || (battle.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    battle.SwitchPokemon(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(BattleReset @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles
      .Include(x => x.Pokemon).ThenInclude(x => x.Pokemon)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is null || (battle.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    battle.Reset(@event);

    foreach (BattlePokemonEntity battler in battle.Pokemon)
    {
      if (battler.Pokemon is null)
      {
        throw new InvalidOperationException("The Pokémon are required.");
      }
      else if (battler.Pokemon.CurrentTrainerId.HasValue)
      {
        _context.BattlePokemon.Remove(battler);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(BattleStarted @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is null || (battle.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    HashSet<string> pokemonIds = @event.PokemonIds.Keys.Select(x => x.Value).ToHashSet();
    PokemonEntity[] pokemon = await _context.Pokemon.Where(x => pokemonIds.Contains(x.StreamId)).ToArrayAsync(cancellationToken);

    battle.Start(pokemon, @event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(BattleUpdated @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is null || (battle.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    battle.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(BattleExperienceGained @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is null || (battle.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    battle.Gain(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(BattleMoveUsed @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles
      .Include(x => x.Pokemon)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is null || (battle.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    battle.UseMove(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(TrainerBattleCreated @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is not null)
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    HashSet<string> championIds = @event.ChampionIds.Select(id => id.Value).ToHashSet();
    TrainerEntity[] champions = await _context.Trainers.Where(x => championIds.Contains(x.StreamId)).ToArrayAsync(cancellationToken);

    HashSet<string> opponentIds = @event.OpponentIds.Select(id => id.Value).ToHashSet();
    TrainerEntity[] opponents = await _context.Trainers.Where(x => opponentIds.Contains(x.StreamId)).ToArrayAsync(cancellationToken);

    battle = new BattleEntity(champions, opponents, @event);
    _context.Battles.Add(battle);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(WildPokemonBattleCreated @event, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _context.Battles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (battle is not null)
    {
      _logger.LogUnexpectedVersion(@event, battle);
      return;
    }

    HashSet<string> championIds = @event.ChampionIds.Select(id => id.Value).ToHashSet();
    TrainerEntity[] champions = await _context.Trainers.Where(x => championIds.Contains(x.StreamId)).ToArrayAsync(cancellationToken);

    HashSet<string> opponentIds = @event.OpponentIds.Select(id => id.Value).ToHashSet();
    PokemonEntity[] opponents = await _context.Pokemon.Where(x => opponentIds.Contains(x.StreamId)).ToArrayAsync(cancellationToken);

    battle = new BattleEntity(champions, opponents, @event);
    _context.Battles.Add(battle);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }
}
