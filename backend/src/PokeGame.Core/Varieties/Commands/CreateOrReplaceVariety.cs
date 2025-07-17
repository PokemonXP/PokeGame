using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties.Models;
using PokeGame.Core.Varieties.Validators;

namespace PokeGame.Core.Varieties.Commands;

internal record CreateOrReplaceVariety(CreateOrReplaceVarietyPayload Payload, Guid? Id) : ICommand<CreateOrReplaceVarietyResult>;

/// <exception cref="SpeciesNotFoundException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceVarietyHandler : ICommandHandler<CreateOrReplaceVariety, CreateOrReplaceVarietyResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISpeciesManager _speciesManager;
  private readonly IVarietyManager _varietyManager;
  private readonly IVarietyQuerier _varietyQuerier;
  private readonly IVarietyRepository _varietyRepository;

  public CreateOrReplaceVarietyHandler(
    IApplicationContext applicationContext,
    ISpeciesManager speciesManager,
    IVarietyManager varietyManager,
    IVarietyQuerier varietyQuerier,
    IVarietyRepository varietyRepository)
  {
    _applicationContext = applicationContext;
    _speciesManager = speciesManager;
    _varietyManager = varietyManager;
    _varietyQuerier = varietyQuerier;
    _varietyRepository = varietyRepository;
  }

  public async Task<CreateOrReplaceVarietyResult> HandleAsync(CreateOrReplaceVariety command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreateOrReplaceVarietyPayload payload = command.Payload;
    new CreateOrReplaceVarietyValidator(uniqueNameSettings).ValidateAndThrow(payload);

    VarietyId varietyId = VarietyId.NewId();
    Variety? variety = null;
    if (command.Id.HasValue)
    {
      varietyId = new(command.Id.Value);
      variety = await _varietyRepository.LoadAsync(varietyId, cancellationToken);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    GenderRatio? genderRatio = payload.GenderRatio.HasValue ? new(payload.GenderRatio.Value) : null;

    bool created = false;
    if (variety is null)
    {
      PokemonSpecies species = await _speciesManager.FindAsync(payload.Species, nameof(payload.Species), cancellationToken);

      variety = new(species, uniqueName, payload.IsDefault, genderRatio, payload.CanChangeForm, actorId, varietyId);
      created = true;
    }
    else
    {
      variety.SetUniqueName(uniqueName, actorId);

      variety.GenderRatio = genderRatio;
      variety.CanChangeForm = payload.CanChangeForm;
    }

    variety.DisplayName = DisplayName.TryCreate(payload.DisplayName);

    variety.Genus = Genus.TryCreate(payload.Genus);
    variety.Description = Description.TryCreate(payload.Description);

    variety.Url = Url.TryCreate(payload.Url);
    variety.Notes = Notes.TryCreate(payload.Notes);

    variety.Update(actorId);
    await _varietyManager.SaveAsync(variety, cancellationToken);

    VarietyModel model = await _varietyQuerier.ReadAsync(variety, cancellationToken);
    return new CreateOrReplaceVarietyResult(model, created);
  }
}
