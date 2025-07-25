using Krakenar.Contracts.Actors;
using Krakenar.Core.Actors;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Inventory;
using PokeGame.Core.Inventory.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class InventoryQuerier : IInventoryQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<InventoryEntity> _inventory;

  public InventoryQuerier(IActorService actorService, PokemonContext context)
  {
    _actorService = actorService;
    _inventory = context.Inventory;
  }

  public async Task<InventoryItemModel?> ReadAsync(Guid trainerId, Guid itemId, CancellationToken cancellationToken)
  {
    InventoryEntity? inventory = await _inventory.AsNoTracking()
      .Include(x => x.Item)
      .SingleOrDefaultAsync(x => x.TrainerUid == trainerId && x.ItemUid == itemId, cancellationToken);
    return inventory is null ? null : await MapAsync(inventory, cancellationToken);
  }

  public async Task<IReadOnlyCollection<InventoryItemModel>> ReadAsync(Guid trainerId, CancellationToken cancellationToken)
  {
    InventoryEntity[] inventory = await _inventory.AsNoTracking()
      .Include(x => x.Item)
      .Where(x => x.TrainerUid == trainerId)
      .ToArrayAsync(cancellationToken);
    return await MapAsync(inventory, cancellationToken);
  }

  private async Task<InventoryItemModel> MapAsync(InventoryEntity entity, CancellationToken cancellationToken)
  {
    return (await MapAsync([entity], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<InventoryItemModel>> MapAsync(IEnumerable<InventoryEntity> entities, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = entities.SelectMany(entity => entity.Item?.GetActorIds() ?? []);
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return entities.Select(mapper.ToInventoryItem).ToList().AsReadOnly();
  }
}
