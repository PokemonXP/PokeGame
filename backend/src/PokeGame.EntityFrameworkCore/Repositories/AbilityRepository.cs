using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Abilities;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class AbilityRepository : Repository, IAbilityRepository
{
  private readonly DbSet<AbilityEntity> _abilities;

  public AbilityRepository(PokemonContext context, IEventStore eventStore) : base(eventStore)
  {
    _abilities = context.Abilities;
  }

  public async Task<Ability?> LoadAsync(AbilityId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Ability>(id.StreamId, cancellationToken);
  }
  public async Task<Ability?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    AbilityId abilityId;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      abilityId = new(id);
      Ability? variety = await LoadAsync(abilityId, cancellationToken);
      if (variety is not null)
      {
        return variety;
      }
    }

    string uniqueNameNormalized = Helper.Normalize(idOrUniqueName);
    string? streamId = await _abilities.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(streamId))
    {
      return null;
    }

    abilityId = new(streamId);
    return await LoadAsync(abilityId, cancellationToken);
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
