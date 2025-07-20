using MediatR;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedBerriesTask : SeedingTask
{
  public override string? Description => "Seeds Berry contents into Krakenar.";
}

internal class SeedBerriesTaskHandler : INotificationHandler<SeedBerriesTask>
{
  private readonly ILogger<SeedBerriesTaskHandler> _logger;

  public SeedBerriesTaskHandler(ILogger<SeedBerriesTaskHandler> logger)
  {
    _logger = logger;
  }

  public async Task Handle(SeedBerriesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<BerryPayload> berries = await CsvHelper.ExtractAsync<BerryPayload>("Game/data/items/berries.csv", cancellationToken);
    // TODO(fpion): implement
  }
}
