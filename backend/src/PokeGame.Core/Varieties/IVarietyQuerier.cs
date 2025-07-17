using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Varieties;

public interface IVarietyQuerier
{
  Task<VarietyId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<VarietyModel> ReadAsync(Variety variety, CancellationToken cancellationToken = default);
  Task<VarietyModel?> ReadAsync(VarietyId id, CancellationToken cancellationToken = default);
  Task<VarietyModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<VarietyModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<VarietyModel>> SearchAsync(SearchVarietiesPayload payload, CancellationToken cancellationToken = default);
}
