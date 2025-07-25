using Logitar.EventSourcing;
using PokeGame.Core.Inventory.Events;
using PokeGame.Core.Items;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Inventory;

public class TrainerInventory : AggregateRoot
{
  public new TrainerInventoryId Id => new(base.Id);
  public TrainerId TrainerId => Id.TrainerId;

  private readonly Dictionary<ItemId, int> _quantities = [];
  public IReadOnlyDictionary<ItemId, int> Quantities => _quantities.AsReadOnly();

  public TrainerInventory() : base()
  {
  }

  public TrainerInventory(Trainer trainer) : base(new TrainerInventoryId(trainer.Id).StreamId)
  {
  }

  public void Add(Item item, int quantity = 1, ActorId? actorId = null) => Add(item.Id, quantity, actorId);
  public void Add(ItemId itemId, int quantity = 1, ActorId? actorId = null)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(quantity, nameof(quantity));

    if (quantity > 0)
    {
      Raise(new InventoryItemAdded(itemId, quantity), actorId);
    }
  }
  protected virtual void Handle(InventoryItemAdded @event)
  {
    _quantities.TryGetValue(@event.ItemId, out int existingQuantity);
    _quantities[@event.ItemId] = existingQuantity + @event.Quantity;
  }

  public void Remove(Item item, int quantity = int.MaxValue, ActorId? actorId = null) => Remove(item.Id, quantity, actorId);
  public void Remove(ItemId itemId, int quantity = int.MaxValue, ActorId? actorId = null)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(quantity, nameof(quantity));

    if (quantity > 0)
    {
      if (_quantities.TryGetValue(itemId, out int existingQuantity) && existingQuantity < quantity)
      {
        quantity = existingQuantity;
      }
      Raise(new InventoryItemRemoved(itemId, quantity), actorId);
    }
  }
  protected virtual void Handle(InventoryItemRemoved @event)
  {
    _quantities.TryGetValue(@event.ItemId, out int existingQuantity);

    if (existingQuantity <= @event.Quantity)
    {
      _quantities.Remove(@event.ItemId);
    }
    else
    {
      _quantities[@event.ItemId] = existingQuantity - @event.Quantity;
    }
  }

  public void Update(Item item, int quantity, ActorId? actorId = null) => Update(item.Id, quantity, actorId);
  public void Update(ItemId itemId, int quantity, ActorId? actorId = null)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(quantity, nameof(quantity));

    if (!_quantities.TryGetValue(itemId, out int existingQuantity) || existingQuantity != quantity)
    {
      Raise(new InventoryItemUpdated(itemId, quantity), actorId);
    }
  }
  protected virtual void Handle(InventoryItemUpdated @event)
  {
    if (@event.Quantity < 1)
    {
      _quantities.Remove(@event.ItemId);
    }
    else
    {
      _quantities[@event.ItemId] = @event.Quantity;
    }
  }
}
