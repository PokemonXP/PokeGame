namespace PokeGame.Core.Inventory.Models;

public record AddInventoryItemPayload
{
  public int? Quantity { get; set; }
}
