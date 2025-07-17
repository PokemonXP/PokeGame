using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Regions.Models;
using PokeGame.Core.Regions.Validators;

namespace PokeGame.Core.Regions.Commands;

internal record UpdateRegion(Guid Id, UpdateRegionPayload Payload) : ICommand<RegionModel?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateRegionHandler : ICommandHandler<UpdateRegion, RegionModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRegionManager _regionManager;
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;

  public UpdateRegionHandler(
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

  public async Task<RegionModel?> HandleAsync(UpdateRegion command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdateRegionPayload payload = command.Payload;
    new UpdateRegionValidator(uniqueNameSettings).ValidateAndThrow(payload);

    RegionId regionId = new(command.Id);
    Region? region = await _regionRepository.LoadAsync(regionId, cancellationToken);
    if (region is null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      region.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      region.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      region.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Url is not null)
    {
      region.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      region.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    region.Update(_applicationContext.ActorId);
    await _regionManager.SaveAsync(region, cancellationToken);

    return await _regionQuerier.ReadAsync(region, cancellationToken);
  }
}
