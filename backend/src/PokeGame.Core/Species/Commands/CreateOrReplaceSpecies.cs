using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Species.Models;
using PokeGame.Core.Species.Validators;

namespace PokeGame.Core.Species.Commands;

internal record CreateOrReplaceSpecies(CreateOrReplaceSpeciesPayload Payload, Guid? Id) : ICommand<CreateOrReplaceSpeciesResult>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceSpeciesHandler : ICommandHandler<CreateOrReplaceSpecies, CreateOrReplaceSpeciesResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISpeciesManager _speciesManager;
  private readonly ISpeciesQuerier _speciesQuerier;
  private readonly ISpeciesRepository _speciesRepository;

  public CreateOrReplaceSpeciesHandler(
    IApplicationContext applicationContext,
    ISpeciesManager speciesManager,
    ISpeciesQuerier speciesQuerier,
    ISpeciesRepository speciesRepository)
  {
    _applicationContext = applicationContext;
    _speciesManager = speciesManager;
    _speciesQuerier = speciesQuerier;
    _speciesRepository = speciesRepository;
  }

  public async Task<CreateOrReplaceSpeciesResult> HandleAsync(CreateOrReplaceSpecies command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreateOrReplaceSpeciesPayload payload = command.Payload;
    new CreateOrReplaceSpeciesValidator(uniqueNameSettings).ValidateAndThrow(payload);

    SpeciesId speciesId = SpeciesId.NewId();
    PokemonSpecies? species = null;
    if (command.Id.HasValue)
    {
      speciesId = new(command.Id.Value);
      species = await _speciesRepository.LoadAsync(speciesId, cancellationToken);
    }

    Number number = new(payload.Number);
    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    Friendship baseFriendship = new(payload.BaseFriendship);
    CatchRate catchRate = new(payload.CatchRate);
    EggCycles eggCycles = new(payload.EggCycles);
    EggGroups eggGroups = new(payload.EggGroups);

    bool created = false;
    if (species is null)
    {
      species = new(number, payload.Category, uniqueName, baseFriendship, catchRate, payload.GrowthRate, eggCycles, eggGroups, actorId, speciesId);
      created = true;
    }
    else
    {
      species.SetUniqueName(uniqueName, actorId);

      species.BaseFriendship = baseFriendship;
      species.CatchRate = catchRate;
      species.GrowthRate = payload.GrowthRate;

      species.EggCycles = eggCycles;
      species.EggGroups = eggGroups;
    }

    species.DisplayName = DisplayName.TryCreate(payload.DisplayName);

    species.Url = Url.TryCreate(payload.Url);
    species.Notes = Notes.TryCreate(payload.Notes);

    // TODO(fpion): RegionalNumbers

    species.Update(actorId);
    await _speciesManager.SaveAsync(species, cancellationToken);

    SpeciesModel model = await _speciesQuerier.ReadAsync(species, cancellationToken);
    return new CreateOrReplaceSpeciesResult(model, created);
  }
}
