using Logitar.EventSourcing;
using PokeGame.Core.Abilities;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class AbilityRepository : Repository, IAbilityRepository
{
  public AbilityRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Ability?> LoadAsync(AbilityId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Ability>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Ability ability, CancellationToken cancellationToken)
  {
    await base.SaveAsync(ability, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Ability> abilities, CancellationToken cancellationToken)
  {
    await base.SaveAsync(abilities, cancellationToken);
  }
}
