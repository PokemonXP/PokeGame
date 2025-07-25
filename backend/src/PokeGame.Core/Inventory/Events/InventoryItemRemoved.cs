using Logitar.EventSourcing;
using PokeGame.Core.Items;

namespace PokeGame.Core.Inventory.Events;

public record InventoryItemRemoved(ItemId ItemId, int Quantity) : DomainEvent;
