using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Battles.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class BattleEvents : IEventHandler<TrainerBattleCreated>, IEventHandler<WildPokemonBattleCreated>
{
  public static void Register(IServiceCollection services)
  {
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
