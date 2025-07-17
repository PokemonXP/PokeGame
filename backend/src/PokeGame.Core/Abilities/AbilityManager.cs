using PokeGame.Core.Abilities.Events;

namespace PokeGame.Core.Abilities;

internal interface IAbilityManager
{
  Task SaveAsync(Ability ability, CancellationToken cancellationToken = default);
}

internal class AbilityManager : IAbilityManager
{
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IAbilityRepository _abilityRepository;

  public AbilityManager(IAbilityQuerier abilityQuerier, IAbilityRepository abilityRepository)
  {
    _abilityQuerier = abilityQuerier;
    _abilityRepository = abilityRepository;
  }

  public async Task SaveAsync(Ability ability, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = ability.Changes.Any(change => change is AbilityCreated || change is AbilityUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      AbilityId? conflictId = await _abilityQuerier.FindIdAsync(ability.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(ability.Id))
      {
        throw new UniqueNameAlreadyUsedException(ability, conflictId.Value);
      }
    }

    await _abilityRepository.SaveAsync(ability, cancellationToken);
  }
}
