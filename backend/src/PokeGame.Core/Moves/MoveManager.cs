using Krakenar.Contracts.Search;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Moves;

internal interface IMoveManager
{
  Task<MoveModel> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<MoveModel>> FindAsync(IEnumerable<string> idOrUniqueNames, string propertyName, CancellationToken cancellationToken = default);
}

internal class MoveManager : IMoveManager
{
  private readonly IMoveQuerier _moveQuerier;

  public MoveManager(IMoveQuerier moveQuerier)
  {
    _moveQuerier = moveQuerier;
  }

  public async Task<MoveModel> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken)
  {
    MoveModel? move = null;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      move = await _moveQuerier.ReadAsync(id, cancellationToken);
    }
    return move ?? await _moveQuerier.ReadAsync(idOrUniqueName, cancellationToken) ?? throw new MoveNotFoundException(idOrUniqueName, propertyName);
  }

  public async Task<IReadOnlyCollection<MoveModel>> FindAsync(IEnumerable<string> idOrUniqueNames, string propertyName, CancellationToken cancellationToken)
  {
    int count = idOrUniqueNames.Count();
    if (count < 1)
    {
      return [];
    }

    SearchResults<MoveModel> results = await _moveQuerier.SearchAsync(new SearchMovesPayload(), cancellationToken);
    int capacity = results.Items.Count;
    Dictionary<Guid, MoveModel> movesById = new(capacity);
    Dictionary<string, MoveModel> movesByUniqueName = new(capacity);
    foreach (MoveModel move in results.Items)
    {
      movesById[move.Id] = move;
      movesByUniqueName[Normalize(move.UniqueName)] = move;
    }

    Dictionary<Guid, MoveModel> moves = new(capacity: count);
    List<string> notFound = new(count);
    foreach (string idOrUniqueName in idOrUniqueNames)
    {
      if (!string.IsNullOrWhiteSpace(idOrUniqueName))
      {
        if ((Guid.TryParse(idOrUniqueName, out Guid id) && movesById.TryGetValue(id, out MoveModel? move))
        || movesByUniqueName.TryGetValue(Normalize(idOrUniqueName), out move))
        {
          moves[move.Id] = move;
        }
        else
        {
          notFound.Add(idOrUniqueName);
        }
      }
    }
    if (notFound.Count > 0)
    {
      throw new MovesNotFoundException(notFound, propertyName);
    }
    return moves.Values.ToList().AsReadOnly();
  }

  private static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
