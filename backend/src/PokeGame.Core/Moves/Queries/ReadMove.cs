using Krakenar.Contracts;
using Krakenar.Core;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Moves.Queries;

internal record ReadMove(Guid? Id, string? UniqueName) : IQuery<MoveModel?>;

/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadMoveHandler : IQueryHandler<ReadMove, MoveModel?>
{
  private readonly IMoveQuerier _moveQuerier;

  public ReadMoveHandler(IMoveQuerier moveQuerier)
  {
    _moveQuerier = moveQuerier;
  }

  public async Task<MoveModel?> HandleAsync(ReadMove query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, MoveModel> moves = new(capacity: 2);

    if (query.Id.HasValue)
    {
      MoveModel? move = await _moveQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (move is not null)
      {
        moves[move.Id] = move;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      MoveModel? move = await _moveQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (move is not null)
      {
        moves[move.Id] = move;
      }
    }

    if (moves.Count > 1)
    {
      throw TooManyResultsException<MoveModel>.ExpectedSingle(moves.Count);
    }

    return moves.SingleOrDefault().Value;
  }
}
