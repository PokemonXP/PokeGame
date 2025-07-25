using PokeGame.Core.Inventory.Models;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;

namespace PokeGame.Api.Models.Game;

public record InventoryItem
{
  public ItemCategory Category { get; set; }
  public string Name { get; set; }
  public string? Description { get; set; }
  public string? Sprite { get; set; }
  public int Quantity { get; set; }

  public InventoryItem() : this(string.Empty)
  {
  }

  public InventoryItem(string name)
  {
    Name = name;
  }

  public InventoryItem(InventoryItemModel model)
  {
    ItemModel item = model.Item;
    Category = item.Category;
    Name = item.DisplayName ?? item.UniqueName;
    Description = item.Description;
    Sprite = item.Sprite;

    Quantity = model.Quantity;
  }
}
