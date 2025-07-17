using MediatR;
using PokeGame.Core.Varieties;
using PokeGame.Core.Varieties.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedVarietiesTask : SeedingTask
{
  public override string? Description => "Seeds Variety contents into Krakenar.";
  public string Language { get; }

  public SeedVarietiesTask(string language)
  {
    Language = language;
  }
}

internal class SeedVarietiesTaskHandler : INotificationHandler<SeedVarietiesTask>
{
  private readonly ILogger<SeedVarietiesTaskHandler> _logger;
  private readonly IVarietyService _varietyService;

  public SeedVarietiesTaskHandler(ILogger<SeedVarietiesTaskHandler> logger, IVarietyService varietyService)
  {
    _logger = logger;
    _varietyService = varietyService;
  }

  public async Task Handle(SeedVarietiesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedVarietyPayload> varieties = await CsvHelper.ExtractAsync<SeedVarietyPayload>("Game/data/varieties.csv", cancellationToken);
    foreach (SeedVarietyPayload variety in varieties)
    {
      CreateOrReplaceVarietyResult result = await _varietyService.CreateOrReplaceAsync(variety, variety.Id, cancellationToken);
      _logger.LogInformation("The variety '{Variety}' was {Status}.", result.Variety, result.Created ? "created" : "updated");
    }
  }
}
