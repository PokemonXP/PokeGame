using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities.Models;
using PokeGame.Core.Abilities.Validators;

namespace PokeGame.Core.Abilities.Commands;

internal record CreateOrReplaceAbility(CreateOrReplaceAbilityPayload Payload, Guid? Id) : ICommand<CreateOrReplaceAbilityResult>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceAbilityHandler : ICommandHandler<CreateOrReplaceAbility, CreateOrReplaceAbilityResult>
{
  private readonly IAbilityManager _abilityManager;
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IAbilityRepository _abilityRepository;
  private readonly IApplicationContext _applicationContext;

  public CreateOrReplaceAbilityHandler(
    IAbilityManager abilityManager,
    IAbilityQuerier abilityQuerier,
    IAbilityRepository abilityRepository,
    IApplicationContext applicationContext)
  {
    _abilityManager = abilityManager;
    _abilityQuerier = abilityQuerier;
    _abilityRepository = abilityRepository;
    _applicationContext = applicationContext;
  }

  public async Task<CreateOrReplaceAbilityResult> HandleAsync(CreateOrReplaceAbility command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreateOrReplaceAbilityPayload payload = command.Payload;
    new CreateOrReplaceAbilityValidator(uniqueNameSettings).ValidateAndThrow(payload);

    AbilityId abilityId = AbilityId.NewId();
    Ability? ability = null;
    if (command.Id.HasValue)
    {
      abilityId = new(command.Id.Value);
      ability = await _abilityRepository.LoadAsync(abilityId, cancellationToken);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);

    bool created = false;
    if (ability is null)
    {
      ability = new(uniqueName, actorId, abilityId);
      created = true;
    }
    else
    {
      ability.SetUniqueName(uniqueName, actorId);
    }

    ability.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    ability.Description = Description.TryCreate(payload.Description);

    ability.Url = Url.TryCreate(payload.Url);
    ability.Notes = Notes.TryCreate(payload.Notes);

    ability.Update(actorId);
    await _abilityManager.SaveAsync(ability, cancellationToken);

    AbilityModel model = await _abilityQuerier.ReadAsync(ability, cancellationToken);
    return new CreateOrReplaceAbilityResult(model, created);
  }
}
