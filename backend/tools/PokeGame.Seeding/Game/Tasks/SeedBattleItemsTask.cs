using MediatR;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedBattleItemsTask : SeedingTask
{
  public override string? Description => "Seeds Battle Item contents into Krakenar.";
}

internal class SeedBattleItemsTaskHandler : INotificationHandler<SeedBattleItemsTask>
{
  private readonly IItemService _itemService;
  private readonly ILogger<SeedBattleItemsTaskHandler> _logger;

  public SeedBattleItemsTaskHandler(IItemService itemService, ILogger<SeedBattleItemsTaskHandler> logger)
  {
    _itemService = itemService;
    _logger = logger;
  }

  public async Task Handle(SeedBattleItemsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedBattleItemPayload> battleItems = await CsvHelper.ExtractAsync<SeedBattleItemPayload>("Game/data/items/battle.csv", cancellationToken);
    foreach (SeedBattleItemPayload battleItem in battleItems)
    {
      CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(battleItem, battleItem.Id, cancellationToken);
      _logger.LogInformation("The battle item '{Item}' was {Status}.", result.Item, result.Created ? "created" : "updated");
    }
  }
}
