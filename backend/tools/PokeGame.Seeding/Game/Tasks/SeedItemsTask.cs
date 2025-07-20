using MediatR;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedItemsTask : SeedingTask
{
  public override string? Description => "Seeds Item contents into Krakenar.";
}

internal class SeedItemsTaskHandler : INotificationHandler<SeedItemsTask>
{
  private readonly ILogger<SeedItemsTaskHandler> _logger;

  public SeedItemsTaskHandler(ILogger<SeedItemsTaskHandler> logger)
  {
    _logger = logger;
  }

  public async Task Handle(SeedItemsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<ItemPayload> items = await CsvHelper.ExtractAsync<ItemPayload>("Game/data/items/other.csv", cancellationToken);
    // TODO(fpion): implement
  }
}
