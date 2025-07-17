namespace PokeGame.Core.Abilities;

public interface IAbilityRepository
{
  Task<Ability?> LoadAsync(AbilityId abilityId, CancellationToken cancellationToken = default);

  Task SaveAsync(Ability ability, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Ability> abilities, CancellationToken cancellationToken = default);
}
