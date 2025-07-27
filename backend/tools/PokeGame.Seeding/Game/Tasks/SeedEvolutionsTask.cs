using MediatR;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Evolutions.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedEvolutionsTask : SeedingTask
{
  public override string? Description => "Seeds Evolution contents into Krakenar.";
}

internal class SeedEvolutionsTaskHandler : INotificationHandler<SeedEvolutionsTask>
{
  private readonly IEvolutionService _evolutionService;
  private readonly ILogger<SeedEvolutionsTaskHandler> _logger;

  public SeedEvolutionsTaskHandler(IEvolutionService evolutionService, ILogger<SeedEvolutionsTaskHandler> logger)
  {
    _evolutionService = evolutionService;
    _logger = logger;
  }

  public async Task Handle(SeedEvolutionsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedEvolutionPayload> evolutions = await CsvHelper.ExtractAsync<SeedEvolutionPayload>("Game/data/evolutions.csv", cancellationToken);
    foreach (SeedEvolutionPayload evolution in evolutions)
    {
      CreateOrReplaceEvolutionResult result = await _evolutionService.CreateOrReplaceAsync(evolution, evolution.Id, cancellationToken);
      _logger.LogInformation("The evolution '{Evolution}' was {Status}.", result.Evolution, result.Created ? "created" : "updated");
    }
  }
}
