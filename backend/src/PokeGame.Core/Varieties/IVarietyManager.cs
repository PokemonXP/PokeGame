using PokeGame.Core.Varieties.Events;

namespace PokeGame.Core.Varieties;

internal interface IVarietyManager
{
  Task SaveAsync(Variety variety, CancellationToken cancellationToken = default);
}

internal class VarietyManager : IVarietyManager
{
  private readonly IVarietyQuerier _varietyQuerier;
  private readonly IVarietyRepository _varietyRepository;

  public VarietyManager(IVarietyQuerier varietyQuerier, IVarietyRepository varietyRepository)
  {
    _varietyQuerier = varietyQuerier;
    _varietyRepository = varietyRepository;
  }

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
