using Krakenar.Contracts.Search;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Varieties;

public interface IVarietyQuerier
{
  Task<VarietyModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<VarietyModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<VarietyModel>> SearchAsync(Guid speciesId, SearchVarietiesPayload payload, CancellationToken cancellationToken = default);
}
