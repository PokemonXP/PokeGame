using Krakenar.Core;
using PokeGame.Core.Species.Models;

namespace PokeGame.Core.Species.Commands;

internal record DeleteSpecies(Guid Id) : ICommand<SpeciesModel?>;

internal class DeleteSpeciesHandler : ICommandHandler<DeleteSpecies, SpeciesModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISpeciesQuerier _speciesQuerier;
  private readonly ISpeciesRepository _speciesRepository;

  public DeleteSpeciesHandler(IApplicationContext applicationContext, ISpeciesQuerier speciesQuerier, ISpeciesRepository speciesRepository)
  {
    _applicationContext = applicationContext;
    _speciesQuerier = speciesQuerier;
    _speciesRepository = speciesRepository;
  }

  public async Task<SpeciesModel?> HandleAsync(DeleteSpecies command, CancellationToken cancellationToken)
  {
    SpeciesId speciesId = new(command.Id);
    PokemonSpecies? species = await _speciesRepository.LoadAsync(speciesId, cancellationToken);
    if (species is null)
    {
      return null;
    }
    SpeciesModel model = await _speciesQuerier.ReadAsync(species, cancellationToken);

    species.Delete(_applicationContext.ActorId);
    await _speciesRepository.SaveAsync(species, cancellationToken);

    return model;
  }
}
