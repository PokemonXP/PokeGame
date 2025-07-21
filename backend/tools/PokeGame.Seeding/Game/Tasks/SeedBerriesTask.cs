using MediatR;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedBerriesTask : SeedingTask
{
  public override string? Description => "Seeds Berry contents into Krakenar.";
}

internal class SeedBerriesTaskHandler : INotificationHandler<SeedBerriesTask>
{
  private readonly IItemService _itemService;
  private readonly ILogger<SeedBerriesTaskHandler> _logger;

  public SeedBerriesTaskHandler(IItemService itemService, ILogger<SeedBerriesTaskHandler> logger)
  {
    _itemService = itemService;
    _logger = logger;
  }

  public async Task Handle(SeedBerriesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedBerryPayload> berries = await CsvHelper.ExtractAsync<SeedBerryPayload>("Game/data/items/berries.csv", cancellationToken);
    foreach (SeedBerryPayload berry in berries)
    {
      CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(berry, berry.Id, cancellationToken);
      _logger.LogInformation("The berry '{Item}' was {Status}.", result.Item, result.Created ? "created" : "updated");
    }
  }
}
