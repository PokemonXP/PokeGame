using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Inventory.Models;

public record InventoryItemModel
{
  public ItemModel Item { get; set; } = new();
  public int Quantity { get; set; }

  public InventoryItemModel()
  {
  }

  public InventoryItemModel(ItemModel item, int quantity = 0)
  {
    Item = item;
    Quantity = quantity;
  }
}
