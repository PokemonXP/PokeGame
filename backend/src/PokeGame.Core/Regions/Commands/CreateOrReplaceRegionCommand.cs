using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Regions.Models;
using PokeGame.Core.Regions.Validators;

namespace PokeGame.Core.Regions.Commands;

internal record CreateOrReplaceRegionCommand(CreateOrReplaceRegionPayload Payload, Guid? Id) : ICommand<CreateOrReplaceRegionResult>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceRegionCommandHandler : ICommandHandler<CreateOrReplaceRegionCommand, CreateOrReplaceRegionResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRegionManager _regionManager;
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;

  public CreateOrReplaceRegionCommandHandler(
    IApplicationContext applicationContext,
    IRegionManager regionManager,
    IRegionQuerier regionQuerier,
    IRegionRepository regionRepository)
  {
    _applicationContext = applicationContext;
    _regionManager = regionManager;
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
  }

  public async Task<CreateOrReplaceRegionResult> HandleAsync(CreateOrReplaceRegionCommand command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreateOrReplaceRegionPayload payload = command.Payload;
    new CreateOrReplaceRegionValidator(uniqueNameSettings).ValidateAndThrow(payload);

    RegionId regionId = RegionId.NewId();
    Region? region = null;
    if (command.Id.HasValue)
    {
      regionId = new(command.Id.Value);
      region = await _regionRepository.LoadAsync(regionId, cancellationToken);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);

    bool created = false;
    if (region is null)
    {
      region = new(uniqueName, actorId, regionId);
      created = true;
    }
    else
    {
      region.SetUniqueName(uniqueName, actorId);
    }

    region.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    region.Description = Description.TryCreate(payload.Description);

    region.Url = Url.TryCreate(payload.Url);
    region.Notes = Notes.TryCreate(payload.Notes);

    region.Update(actorId);
    await _regionManager.SaveAsync(region, cancellationToken);

    RegionModel model = await _regionQuerier.ReadAsync(region, cancellationToken);
    return new CreateOrReplaceRegionResult(model, created);
  }
}
