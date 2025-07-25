namespace PokeGame.Core.Inventory.Models;

public record InventoryQuantityPayload
{
  public int Quantity { get; set; }

  public InventoryQuantityPayload()
  {
  }

  public InventoryQuantityPayload(int quantity)
  {
    Quantity = quantity;
  }
}
