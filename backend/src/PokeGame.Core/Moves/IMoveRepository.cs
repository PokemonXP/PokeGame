namespace PokeGame.Core.Moves;

public interface IMoveRepository
{
  Task<Move?> LoadAsync(MoveId moveId, CancellationToken cancellationToken = default);
  Task<Move?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Move>> LoadAsync(IEnumerable<MoveId> moveIds, CancellationToken cancellationToken = default);

  Task SaveAsync(Move move, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Move> moves, CancellationToken cancellationToken = default);
}
