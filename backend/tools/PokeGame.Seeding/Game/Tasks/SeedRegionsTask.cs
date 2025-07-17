using MediatR;
using PokeGame.Core.Regions;
using PokeGame.Core.Regions.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedRegionsTask : SeedingTask
{
  public override string? Description => "Seeds Region contents into Krakenar.";
  public string Language { get; }

  public SeedRegionsTask(string language)
  {
    Language = language;
  }
}

internal class SeedRegionsTaskHandler : INotificationHandler<SeedRegionsTask>
{
  private readonly ILogger<SeedRegionsTaskHandler> _logger;
  private readonly IRegionService _regionService;

  public SeedRegionsTaskHandler(ILogger<SeedRegionsTaskHandler> logger, IRegionService regionService)
  {
    _logger = logger;
    _regionService = regionService;
  }

  public async Task Handle(SeedRegionsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedRegionPayload> regions = await CsvHelper.ExtractAsync<SeedRegionPayload>("Game/data/regions.csv", cancellationToken);
    foreach (SeedRegionPayload region in regions)
    {
      CreateOrReplaceRegionResult result = await _regionService.CreateOrReplaceAsync(region, region.Id, cancellationToken);
      _logger.LogInformation("The region '{Region}' was {Status}.", result.Region, result.Created ? "created" : "updated");
    }
  }
}
