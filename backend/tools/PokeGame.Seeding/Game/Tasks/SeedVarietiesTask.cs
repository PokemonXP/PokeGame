using MediatR;
using PokeGame.Core.Varieties;
using PokeGame.Core.Varieties.Models;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Tools.Shared;
using PokeGame.Tools.Shared.Models;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedVarietiesTask : SeedingTask
{
  public override string? Description => "Seeds Variety contents into Krakenar.";
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
    CsvManager csv = new([new SeedVarietyPayload.Map(), new SeedVarietyMovePayload.Map()]);
    IReadOnlyCollection<SeedVarietyMovePayload> varietyMoves = await csv.ExtractAsync<SeedVarietyMovePayload>("Game/data/varieties/moves.csv", cancellationToken);
    Dictionary<string, SeedVarietyMovePayload[]> groupedMoves = varietyMoves.GroupBy(x => Normalize(x.Variety)).ToDictionary(x => x.Key, x => x.ToArray());

    IReadOnlyCollection<SeedVarietyPayload> varieties = await csv.ExtractAsync<SeedVarietyPayload>("Game/data/varieties.csv", cancellationToken);

    foreach (SeedVarietyPayload variety in varieties)
    {
      if (groupedMoves.TryGetValue(variety.Id.ToString(), out SeedVarietyMovePayload[]? moves))
      {
        foreach (SeedVarietyMovePayload move in moves)
        {
          variety.Moves.Add(new VarietyMovePayload(move.Move, move.Level));
        }
      }
      if (groupedMoves.TryGetValue(Normalize(variety.UniqueName), out moves))
      {
        foreach (SeedVarietyMovePayload move in moves)
        {
          variety.Moves.Add(new VarietyMovePayload(move.Move, move.Level));
        }
      }

      CreateOrReplaceVarietyResult result = await _varietyService.CreateOrReplaceAsync(variety, variety.Id, cancellationToken);
      _logger.LogInformation("The variety '{Variety}' was {Status}.", result.Variety, result.Created ? "created" : "updated");
    }
  }
  private static string Normalize(string value) => value.Trim().ToLowerInvariant();
}
