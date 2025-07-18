using PokeGame.Core.Moves;
using PokeGame.Core.Varieties.Events;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Varieties;

internal interface IVarietyManager
{
  Task<IReadOnlyDictionary<MoveId, int?>> FindMovesAsync(IEnumerable<VarietyMovePayload> payloads, string propertyName, CancellationToken cancellationToken = default);
  Task SaveAsync(Variety variety, CancellationToken cancellationToken = default);
}

internal class VarietyManager : IVarietyManager
{
  private readonly IMoveQuerier _moveQuerier;
  private readonly IVarietyQuerier _varietyQuerier;
  private readonly IVarietyRepository _varietyRepository;

  public VarietyManager(IMoveQuerier moveQuerier, IVarietyQuerier varietyQuerier, IVarietyRepository varietyRepository)
  {
    _moveQuerier = moveQuerier;
    _varietyQuerier = varietyQuerier;
    _varietyRepository = varietyRepository;
  }

  public async Task<IReadOnlyDictionary<MoveId, int?>> FindMovesAsync(IEnumerable<VarietyMovePayload> payloads, string propertyName, CancellationToken cancellationToken)
  {
    int capacity = payloads.Count();
    Dictionary<MoveId, int?> moves = new(capacity);

    if (capacity > 0)
    {
      IReadOnlyCollection<MoveKey> keys = await _moveQuerier.GetKeysAsync(cancellationToken);
      Dictionary<Guid, MoveId> ids = new(capacity: keys.Count);
      Dictionary<string, MoveId> uniqueNames = new(capacity: keys.Count);
      foreach (MoveKey key in keys)
      {
        ids[key.Id] = key.MoveId;
        uniqueNames[Normalize(key.UniqueName)] = key.MoveId;
      }

      List<string> missing = new(capacity);
      foreach (VarietyMovePayload payload in payloads)
      {
        if (!string.IsNullOrWhiteSpace(payload.Move))
        {
          if (Guid.TryParse(payload.Move, out Guid id) && ids.TryGetValue(id, out MoveId moveId)
            || uniqueNames.TryGetValue(Normalize(payload.Move), out moveId))
          {
            moves[moveId] = payload.Level;
          }
          else
          {
            missing.Add(payload.Move);
          }
        }
      }

      if (missing.Count > 0)
      {
        throw new MovesNotFoundException(missing, propertyName);
      }
    }

    return moves.AsReadOnly();
  }
  private static string Normalize(string value) => value.Trim().ToLowerInvariant();

  public async Task SaveAsync(Variety variety, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = variety.Changes.Any(change => change is VarietyCreated || change is VarietyUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      VarietyId? conflictId = await _varietyQuerier.FindIdAsync(variety.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(variety.Id))
      {
        throw new UniqueNameAlreadyUsedException(variety, conflictId.Value);
      }
    }

    await _varietyRepository.SaveAsync(variety, cancellationToken);
  }
}
