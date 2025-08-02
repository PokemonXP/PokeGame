namespace PokeGame.Core.Varieties;

public interface IVarietyRepository
{
  Task<Variety?> LoadAsync(VarietyId varietyId, CancellationToken cancellationToken = default);
  Task<Variety?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Variety>> LoadAsync(IEnumerable<VarietyId> varietyIds, CancellationToken cancellationToken = default);

  Task SaveAsync(Variety variety, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Variety> varieties, CancellationToken cancellationToken = default);
}
