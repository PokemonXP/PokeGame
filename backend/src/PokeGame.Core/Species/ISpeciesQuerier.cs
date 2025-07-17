using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Species.Models;

namespace PokeGame.Core.Species;

public interface ISpeciesQuerier
{
  Task<SpeciesId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<SpeciesModel> ReadAsync(PokemonSpecies species, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(SpeciesId id, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(int number, Guid? regionId = null, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<SpeciesModel>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken = default);
}
