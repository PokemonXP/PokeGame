using PokeGame.Core.Moves.Events;

namespace PokeGame.Core.Moves;

internal interface IMoveManager
{
  Task SaveAsync(Move move, CancellationToken cancellationToken = default);
}

internal class MoveManager : IMoveManager
{
  private readonly IMoveQuerier _moveQuerier;
  private readonly IMoveRepository _moveRepository;

  public MoveManager(IMoveQuerier moveQuerier, IMoveRepository moveRepository)
  {
    _moveQuerier = moveQuerier;
    _moveRepository = moveRepository;
  }

  public async Task SaveAsync(Move move, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = move.Changes.Any(change => change is MoveCreated || change is MoveUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      MoveId? conflictId = await _moveQuerier.FindIdAsync(move.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(move.Id))
      {
        throw new UniqueNameAlreadyUsedException(move, conflictId.Value);
      }
    }

    await _moveRepository.SaveAsync(move, cancellationToken);
  }
}
