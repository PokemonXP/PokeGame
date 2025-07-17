using MediatR;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedSpeciesTask : SeedingTask
{
  public override string? Description => "Seeds Species contents into Krakenar.";
  public string Language { get; }

  public SeedSpeciesTask(string language)
  {
    Language = language;
  }
}

internal class SeedSpeciesTaskHandler : INotificationHandler<SeedSpeciesTask>
{
  private readonly ILogger<SeedSpeciesTaskHandler> _logger;
  private readonly ISpeciesService _speciesService;

  public SeedSpeciesTaskHandler(ILogger<SeedSpeciesTaskHandler> logger, ISpeciesService speciesService)
  {
    _logger = logger;
    _speciesService = speciesService;
  }

  public async Task Handle(SeedSpeciesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedSpeciesPayload> speciesList = await CsvHelper.ExtractAsync<SeedSpeciesPayload>("Game/data/species.csv", cancellationToken);
    foreach (SeedSpeciesPayload species in speciesList)
    {
      CreateOrReplaceSpeciesResult result = await _speciesService.CreateOrReplaceAsync(species, species.Id, cancellationToken);
      _logger.LogInformation("The species '{Species}' was {Status}.", result.Species, result.Created ? "created" : "updated");
    }
  }
}
