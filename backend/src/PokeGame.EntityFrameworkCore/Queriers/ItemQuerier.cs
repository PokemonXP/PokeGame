using Krakenar.Contracts.Actors;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class ItemQuerier : IItemQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ItemEntity> _items;

  public ItemQuerier(IActorService actorService, PokemonContext context)
  {
    _actorService = actorService;
    _items = context.Items;
  }

  public async Task<ItemModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _items.AsNoTracking()
      .Include(x => x.Move)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return item is null ? null : await MapAsync(item, cancellationToken);
  }
  public async Task<ItemModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    ItemEntity? item = await _items.AsNoTracking()
      .Include(x => x.Move)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return item is null ? null : await MapAsync(item, cancellationToken);
  }

  private async Task<ItemModel> MapAsync(ItemEntity item, CancellationToken cancellationToken)
  {
    return (await MapAsync([item], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ItemModel>> MapAsync(IEnumerable<ItemEntity> items, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = items.SelectMany(item => item.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return items.Select(mapper.ToItem).ToList().AsReadOnly();
  }
}
