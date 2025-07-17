using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Moves;

public interface IMoveQuerier
{
  Task<MoveId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<MoveModel> ReadAsync(Move move, CancellationToken cancellationToken = default);
  Task<MoveModel?> ReadAsync(MoveId id, CancellationToken cancellationToken = default);
  Task<MoveModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<MoveModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<MoveModel>> SearchAsync(SearchMovesPayload payload, CancellationToken cancellationToken = default);
}
