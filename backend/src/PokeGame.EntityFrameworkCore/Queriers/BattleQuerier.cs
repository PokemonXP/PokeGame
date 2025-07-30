using Krakenar.Contracts.Actors;
using Krakenar.Core.Actors;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Battles;
using PokeGame.Core.Battles.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class BattleQuerier : IBattleQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<BattleEntity> _battles;
  private readonly DbSet<PokemonEntity> _pokemon;

  public BattleQuerier(IActorService actorService, PokemonContext context)
  {
    _actorService = actorService;
    _battles = context.Battles;
    _pokemon = context.Pokemon;
  }

  public async Task<BattleModel> ReadAsync(Battle battle, CancellationToken cancellationToken)
  {
    return await ReadAsync(battle.Id, cancellationToken) ?? throw new InvalidOperationException($"The battle entity 'StreamId={battle.Id}' was not found.");
  }
  public async Task<BattleModel?> ReadAsync(BattleId id, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _battles.AsNoTracking()
      .Include(x => x.Pokemon)
      .Include(x => x.Trainers).ThenInclude(x => x.Trainer)
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    await FillAsync(battle, cancellationToken);
    return await MapAsync(battle, cancellationToken);
  }
  public async Task<BattleModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    BattleEntity? battle = await _battles.AsNoTracking()
      .Include(x => x.Pokemon)
      .Include(x => x.Trainers).ThenInclude(x => x.Trainer)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    await FillAsync(battle, cancellationToken);
    return await MapAsync(battle, cancellationToken);
  }

  private async Task FillAsync(BattleEntity battle, CancellationToken cancellationToken)
  {
    if (battle.Pokemon.Count > 0)
    {
      HashSet<int> pokemonIds = battle.Pokemon.Select(x => x.PokemonId).ToHashSet();
      Dictionary<int, PokemonEntity> pokemon = await _pokemon.AsNoTracking()
        .Include(x => x.Form).ThenInclude(x => x!.Abilities).ThenInclude(x => x.Ability)
        .Include(x => x.Form).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species)
        .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
        .Where(x => pokemonIds.Contains(x.PokemonId))
        .ToDictionaryAsync(x => x.PokemonId, x => x, cancellationToken);
      foreach (BattlePokemonEntity entity in battle.Pokemon)
      {
        entity.Pokemon = pokemon[entity.PokemonId];
      }
    }
  }

  private async Task<BattleModel> MapAsync(BattleEntity battle, CancellationToken cancellationToken)
  {
    return (await MapAsync([battle], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<BattleModel>> MapAsync(IEnumerable<BattleEntity> battles, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = battles.SelectMany(battle => battle.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return battles.Select(mapper.ToBattle).ToList().AsReadOnly();
  }
}
