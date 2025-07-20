using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Moves;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class MoveRepository : Repository, IMoveRepository
{
  private readonly DbSet<MoveEntity> _moves;

  public MoveRepository(PokemonContext context, IEventStore eventStore) : base(eventStore)
  {
    _moves = context.Moves;
  }

  public async Task<Move?> LoadAsync(MoveId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Move>(id.StreamId, cancellationToken);
  }

  public async Task<Move?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    MoveId moveId;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      moveId = new(id);
      Move? variety = await LoadAsync(moveId, cancellationToken);
      if (variety is not null)
      {
        return variety;
      }
    }

    string uniqueNameNormalized = Helper.Normalize(idOrUniqueName);
    string? streamId = await _moves.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(streamId))
    {
      return null;
    }

    moveId = new(streamId);
    return await LoadAsync(moveId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Move>> LoadAsync(IEnumerable<MoveId> ids, CancellationToken cancellationToken)
  {
    IEnumerable<StreamId> streamIds = ids.Select(id => id.StreamId);
    return await LoadAsync<Move>(streamIds, cancellationToken);
  }

  public async Task SaveAsync(Move move, CancellationToken cancellationToken)
  {
    await base.SaveAsync(move, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Move> moves, CancellationToken cancellationToken)
  {
    await base.SaveAsync(moves, cancellationToken);
  }
}
