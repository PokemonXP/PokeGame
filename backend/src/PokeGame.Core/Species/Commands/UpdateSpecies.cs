using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Species.Models;
using PokeGame.Core.Species.Validators;

namespace PokeGame.Core.Species.Commands;

internal record UpdateSpecies(Guid Id, UpdateSpeciesPayload Payload) : ICommand<SpeciesModel?>;

/// <exception cref="NumberAlreadyUsedException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateSpeciesHandler : ICommandHandler<UpdateSpecies, SpeciesModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISpeciesManager _speciesManager;
  private readonly ISpeciesQuerier _speciesQuerier;
  private readonly ISpeciesRepository _speciesRepository;

  public UpdateSpeciesHandler(
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

  public async Task<SpeciesModel?> HandleAsync(UpdateSpecies command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdateSpeciesPayload payload = command.Payload;
    new UpdateSpeciesValidator(uniqueNameSettings).ValidateAndThrow(payload);

    SpeciesId speciesId = new(command.Id);
    PokemonSpecies? species = await _speciesRepository.LoadAsync(speciesId, cancellationToken);
    if (species is null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      species.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      species.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }

    if (payload.BaseFriendship.HasValue)
    {
      species.BaseFriendship = new Friendship(payload.BaseFriendship.Value);
    }
    if (payload.CatchRate.HasValue)
    {
      species.CatchRate = new CatchRate(payload.CatchRate.Value);
    }
    if (payload.GrowthRate.HasValue)
    {
      species.GrowthRate = payload.GrowthRate.Value;
    }

    if (payload.EggCycles.HasValue)
    {
      species.EggCycles = new EggCycles(payload.EggCycles.Value);
    }
    if (payload.EggGroups is not null)
    {
      species.EggGroups = new EggGroups(payload.EggGroups);
    }

    if (payload.Url is not null)
    {
      species.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      species.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    // TODO(fpion): RegionalNumbers

    species.Update(_applicationContext.ActorId);
    await _speciesManager.SaveAsync(species, cancellationToken);

    return await _speciesQuerier.ReadAsync(species, cancellationToken);
  }
}
