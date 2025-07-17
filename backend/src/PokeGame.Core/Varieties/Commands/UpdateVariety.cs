using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Varieties.Models;
using PokeGame.Core.Varieties.Validators;

namespace PokeGame.Core.Varieties.Commands;

internal record UpdateVariety(Guid Id, UpdateVarietyPayload Payload) : ICommand<VarietyModel?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateVarietyHandler : ICommandHandler<UpdateVariety, VarietyModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IVarietyManager _varietyManager;
  private readonly IVarietyQuerier _varietyQuerier;
  private readonly IVarietyRepository _varietyRepository;

  public UpdateVarietyHandler(
    IApplicationContext applicationContext,
    IVarietyManager varietyManager,
    IVarietyQuerier varietyQuerier,
    IVarietyRepository varietyRepository)
  {
    _applicationContext = applicationContext;
    _varietyManager = varietyManager;
    _varietyQuerier = varietyQuerier;
    _varietyRepository = varietyRepository;
  }

  public async Task<VarietyModel?> HandleAsync(UpdateVariety command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdateVarietyPayload payload = command.Payload;
    new UpdateVarietyValidator(uniqueNameSettings).ValidateAndThrow(payload);

    VarietyId varietyId = new(command.Id);
    Variety? variety = await _varietyRepository.LoadAsync(varietyId, cancellationToken);
    if (variety is null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      variety.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      variety.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }

    if (payload.Genus is not null)
    {
      variety.Genus = Genus.TryCreate(payload.Genus.Value);
    }
    if (payload.Description is not null)
    {
      variety.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.GenderRatio is not null)
    {
      variety.GenderRatio = payload.GenderRatio.Value.HasValue ? new GenderRatio(payload.GenderRatio.Value.Value) : null;
    }

    if (payload.CanChangeForm.HasValue)
    {
      variety.CanChangeForm = payload.CanChangeForm.Value;
    }

    if (payload.Url is not null)
    {
      variety.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      variety.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    variety.Update(_applicationContext.ActorId);
    await _varietyManager.SaveAsync(variety, cancellationToken);

    return await _varietyQuerier.ReadAsync(variety, cancellationToken);
  }
}
