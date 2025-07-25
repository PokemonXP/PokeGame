using PokeGame.Core.Inventory.Models;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Api.Models.Game;

public record Inventory
{
  public int Money { get; set; }
  public List<InventoryItem> Items { get; set; }

  public Inventory()
  {
    Items = [];
  }

  public Inventory(TrainerModel trainer, IEnumerable<InventoryItemModel>? items = null) : this()
  {
    Money = trainer.Money;

    if (items is not null)
    {
      Items.AddRange(items.Select(item => new InventoryItem(item)));
    }
  }
}
