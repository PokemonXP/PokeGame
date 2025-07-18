using PokeGame.Core.Abilities;
using PokeGame.Core.Forms.Events;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms;

internal interface IFormManager
{
  Task<FormAbilities> FindAbilitiesAsync(FormAbilitiesPayload payload, string propertyName, CancellationToken cancellationToken = default);
  Task SaveAsync(Form form, CancellationToken cancellationToken = default);
}

internal class FormManager : IFormManager
{
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IAbilityRepository _abilityRepository;
  private readonly IFormQuerier _formQuerier;
  private readonly IFormRepository _formRepository;

  public FormManager(IAbilityQuerier abilityQuerier, IAbilityRepository abilityRepository, IFormQuerier formQuerier, IFormRepository formRepository)
  {
    _abilityQuerier = abilityQuerier;
    _abilityRepository = abilityRepository;
    _formQuerier = formQuerier;
    _formRepository = formRepository;
  }

  public async Task<FormAbilities> FindAbilitiesAsync(FormAbilitiesPayload payload, string propertyName, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(payload.Secondary) && string.IsNullOrWhiteSpace(payload.Hidden))
    {
      Ability ability = await _abilityRepository.LoadAsync(payload.Primary, cancellationToken)
        ?? throw new AbilityNotFoundException(payload.Primary, string.Join('.', propertyName, nameof(payload.Primary)));
      return new FormAbilities(ability);
    }

    IReadOnlyCollection<AbilityKey> keys = await _abilityQuerier.GetKeysAsync(cancellationToken);
    Dictionary<Guid, AbilityId> ids = new(capacity: keys.Count);
    Dictionary<string, AbilityId> uniqueNames = new(capacity: keys.Count);
    foreach (AbilityKey key in keys)
    {
      ids[key.Id] = key.AbilityId;
      uniqueNames[Normalize(key.UniqueName)] = key.AbilityId;
    }

    return new FormAbilities(
      FindAbilityId(payload.Primary, ids, uniqueNames, string.Join('.', propertyName, nameof(payload.Primary))),
      string.IsNullOrWhiteSpace(payload.Secondary) ? null : FindAbilityId(payload.Secondary, ids, uniqueNames, string.Join('.', propertyName, nameof(payload.Secondary))),
      string.IsNullOrWhiteSpace(payload.Hidden) ? null : FindAbilityId(payload.Hidden, ids, uniqueNames, string.Join('.', propertyName, nameof(payload.Hidden))));
  }
  private static AbilityId FindAbilityId(
    string idOrUniqueName,
    IReadOnlyDictionary<Guid, AbilityId> ids,
    IReadOnlyDictionary<string, AbilityId> uniqueNames,
    string propertyName)
  {
    if ((Guid.TryParse(idOrUniqueName, out Guid id) && ids.TryGetValue(id, out AbilityId abilityId))
      || uniqueNames.TryGetValue(Normalize(idOrUniqueName), out abilityId))
    {
      return abilityId;
    }
    throw new AbilityNotFoundException(idOrUniqueName, propertyName);
  }
  private static string Normalize(string value) => value.Trim().ToLowerInvariant();

  public async Task SaveAsync(Form form, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = form.Changes.Any(change => change is FormCreated || change is FormUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      FormId? conflictId = await _formQuerier.FindIdAsync(form.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(form.Id))
      {
        throw new UniqueNameAlreadyUsedException(form, conflictId.Value);
      }
    }

    await _formRepository.SaveAsync(form, cancellationToken);
  }
}
