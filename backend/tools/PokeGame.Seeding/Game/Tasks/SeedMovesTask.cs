using MediatR;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;
using PokeGame.Tools.Shared;
using PokeGame.Tools.Shared.Models;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedMovesTask : SeedingTask
{
  public override string? Description => "Seeds Move contents into Krakenar.";
}

internal class SeedMovesTaskHandler : INotificationHandler<SeedMovesTask>
{
  private readonly ILogger<SeedMovesTaskHandler> _logger;
  private readonly IMoveService _moveService;

  public SeedMovesTaskHandler(ILogger<SeedMovesTaskHandler> logger, IMoveService moveService)
  {
    _logger = logger;
    _moveService = moveService;
  }

  public async Task Handle(SeedMovesTask task, CancellationToken cancellationToken)
  {
    CsvManager csv = new([new SeedMovePayload.Map()]);
    IReadOnlyCollection<SeedMovePayload> moves = await csv.ExtractAsync<SeedMovePayload>("Game/data/moves.csv", cancellationToken);
    foreach (SeedMovePayload move in moves)
    {
      CreateOrReplaceMoveResult result = await _moveService.CreateOrReplaceAsync(move, move.Id, cancellationToken);
      _logger.LogInformation("The move '{Move}' was {Status}.", result.Move, result.Created ? "created" : "updated");
    }
  }
}
