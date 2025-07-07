using Krakenar.Contracts.Search;
using PokeGame.Core.Species.Models;

namespace PokeGame.Core.Species;

public interface ISpeciesQuerier
{
  Task<SpeciesModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(int number, Guid? regionId = null, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<SpeciesModel>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken = default);
}
