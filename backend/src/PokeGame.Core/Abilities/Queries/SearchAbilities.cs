using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Abilities.Models;

namespace PokeGame.Core.Abilities.Queries;

internal record SearchAbilities(SearchAbilitiesPayload Payload) : IQuery<SearchResults<AbilityModel>>;

internal class SearchAbilitiesHandler : IQueryHandler<SearchAbilities, SearchResults<AbilityModel>>
{
  private readonly IAbilityQuerier _abilityQuerier;

  public SearchAbilitiesHandler(IAbilityQuerier abilityQuerier)
  {
    _abilityQuerier = abilityQuerier;
  }

  public async Task<SearchResults<AbilityModel>> HandleAsync(SearchAbilities query, CancellationToken cancellationToken)
  {
    return await _abilityQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
