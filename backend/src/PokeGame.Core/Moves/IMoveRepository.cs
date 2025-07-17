namespace PokeGame.Core.Moves;

public interface IMoveRepository
{
  Task<Move?> LoadAsync(MoveId moveId, CancellationToken cancellationToken = default);

  Task SaveAsync(Move move, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Move> moves, CancellationToken cancellationToken = default);
}
