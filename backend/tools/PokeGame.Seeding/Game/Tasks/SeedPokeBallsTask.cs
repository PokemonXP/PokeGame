using MediatR;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedPokeBallsTask : SeedingTask
{
  public override string? Description => "Seeds PokeBall contents into Krakenar.";
}

internal class SeedPokeBallsTaskHandler : INotificationHandler<SeedPokeBallsTask>
{
  private readonly ILogger<SeedPokeBallsTaskHandler> _logger;

  public SeedPokeBallsTaskHandler(ILogger<SeedPokeBallsTaskHandler> logger)
  {
    _logger = logger;
  }

  public async Task Handle(SeedPokeBallsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<PokeBallPayload> pokeBalls = await CsvHelper.ExtractAsync<PokeBallPayload>("Game/data/items/poke_balls.csv", cancellationToken);
    // TODO(fpion): implement
  }
}
