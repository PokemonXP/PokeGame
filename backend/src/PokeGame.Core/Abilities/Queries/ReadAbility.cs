using Krakenar.Contracts;
using Krakenar.Core;
using PokeGame.Core.Abilities.Models;

namespace PokeGame.Core.Abilities.Queries;

internal record ReadAbility(Guid? Id, string? UniqueName) : IQuery<AbilityModel?>;

/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadAbilityHandler : IQueryHandler<ReadAbility, AbilityModel?>
{
  private readonly IAbilityQuerier _abilityQuerier;

  public ReadAbilityHandler(IAbilityQuerier abilityQuerier)
  {
    _abilityQuerier = abilityQuerier;
  }

  public async Task<AbilityModel?> HandleAsync(ReadAbility query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, AbilityModel> abilities = new(capacity: 2);

    if (query.Id.HasValue)
    {
      AbilityModel? ability = await _abilityQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (ability is not null)
      {
        abilities[ability.Id] = ability;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      AbilityModel? ability = await _abilityQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (ability is not null)
      {
        abilities[ability.Id] = ability;
      }
    }

    if (abilities.Count > 1)
    {
      throw TooManyResultsException<AbilityModel>.ExpectedSingle(abilities.Count);
    }

    return abilities.SingleOrDefault().Value;
  }
}
