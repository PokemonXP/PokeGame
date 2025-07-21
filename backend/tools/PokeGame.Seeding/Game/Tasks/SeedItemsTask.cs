using MediatR;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedItemsTask : SeedingTask
{
  public override string? Description => "Seeds Item contents into Krakenar.";
}

internal class SeedItemsTaskHandler : INotificationHandler<SeedItemsTask>
{
  private readonly IItemService _itemService;
  private readonly ILogger<SeedItemsTaskHandler> _logger;

  public SeedItemsTaskHandler(IItemService itemService, ILogger<SeedItemsTaskHandler> logger)
  {
    _itemService = itemService;
    _logger = logger;
  }

  public async Task Handle(SeedItemsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedItemPayload> items = await CsvHelper.ExtractAsync<SeedItemPayload>("Game/data/items/other.csv", cancellationToken);
    foreach (SeedItemPayload item in items)
    {
      SetProperties(item);
      CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(item, item.Id, cancellationToken);
      _logger.LogInformation("The item '{Item}' was {Status}.", result.Item, result.Created ? "created" : "updated");
    }
  }

  private static void SetProperties(SeedItemPayload item)
  {
    switch (item.Category)
    {
      case ItemCategory.BattleItem:
        item.BattleItem = new BattleItemPropertiesModel();
        break;
      case ItemCategory.Berry:
        item.Berry = new BerryPropertiesModel();
        break;
      case ItemCategory.KeyItem:
        item.KeyItem = new KeyItemPropertiesModel();
        break;
      case ItemCategory.Material:
        item.Material = new MaterialPropertiesModel();
        break;
      case ItemCategory.Medicine:
        item.Medicine = new MedicinePropertiesModel();
        break;
      case ItemCategory.OtherItem:
        item.OtherItem = new OtherItemPropertiesModel();
        break;
      case ItemCategory.PokeBall:
        item.PokeBall = new PokeBallPropertiesModel();
        break;
      case ItemCategory.TechnicalMachine:
        item.TechnicalMachine = new TechnicalMachinePropertiesPayload();
        break;
      case ItemCategory.Treasure:
        item.Treasure = new TreasurePropertiesModel();
        break;
      default:
        throw new ItemCategoryNotSupportedException(item.Category);
    }
  }
}
