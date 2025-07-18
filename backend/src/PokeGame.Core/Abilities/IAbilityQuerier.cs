using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Abilities.Models;

namespace PokeGame.Core.Abilities;

public interface IAbilityQuerier
{
  Task<AbilityId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<AbilityKey>> GetKeysAsync(CancellationToken cancellationToken = default);

  Task<AbilityModel> ReadAsync(Ability ability, CancellationToken cancellationToken = default);
  Task<AbilityModel?> ReadAsync(AbilityId id, CancellationToken cancellationToken = default);
  Task<AbilityModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<AbilityModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<AbilityModel>> SearchAsync(SearchAbilitiesPayload payload, CancellationToken cancellationToken = default);
}
