using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Moves.Queries;

internal record SearchMoves(SearchMovesPayload Payload) : IQuery<SearchResults<MoveModel>>;

internal class SearchMovesHandler : IQueryHandler<SearchMoves, SearchResults<MoveModel>>
{
  private readonly IMoveQuerier _moveQuerier;

  public SearchMovesHandler(IMoveQuerier moveQuerier)
  {
    _moveQuerier = moveQuerier;
  }

  public async Task<SearchResults<MoveModel>> HandleAsync(SearchMoves query, CancellationToken cancellationToken)
  {
    return await _moveQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
