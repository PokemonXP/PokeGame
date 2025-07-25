using Logitar.EventSourcing;
using PokeGame.Core.Items;

namespace PokeGame.Core.Inventory.Events;

public record InventoryItemUpdated(ItemId ItemId, int Quantity) : DomainEvent;
