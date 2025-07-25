using Logitar.EventSourcing;
using PokeGame.Core.Items;

namespace PokeGame.Core.Inventory.Events;

public record InventoryItemAdded(ItemId ItemId, int Quantity) : DomainEvent;
