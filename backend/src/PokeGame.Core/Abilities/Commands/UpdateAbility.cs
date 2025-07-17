using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities.Models;
using PokeGame.Core.Abilities.Validators;

namespace PokeGame.Core.Abilities.Commands;

internal record UpdateAbility(Guid Id, UpdateAbilityPayload Payload) : ICommand<AbilityModel?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateAbilityHandler : ICommandHandler<UpdateAbility, AbilityModel?>
{
  private readonly IAbilityManager _abilityManager;
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IAbilityRepository _abilityRepository;
  private readonly IApplicationContext _applicationContext;

  public UpdateAbilityHandler(
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

  public async Task<AbilityModel?> HandleAsync(UpdateAbility command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdateAbilityPayload payload = command.Payload;
    new UpdateAbilityValidator(uniqueNameSettings).ValidateAndThrow(payload);

    AbilityId abilityId = new(command.Id);
    Ability? ability = await _abilityRepository.LoadAsync(abilityId, cancellationToken);
    if (ability is null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      ability.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      ability.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      ability.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Url is not null)
    {
      ability.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      ability.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    ability.Update(_applicationContext.ActorId);
    await _abilityManager.SaveAsync(ability, cancellationToken);

    return await _abilityQuerier.ReadAsync(ability, cancellationToken);
  }
}
