using PokeGame.Core.Inventory.Events;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class InventoryEntity
{
  public TrainerEntity? Trainer { get; private set; }
  public int TrainerId { get; private set; }
  public Guid TrainerUid { get; private set; }

  public ItemEntity? Item { get; private set; }
  public int ItemId { get; private set; }
  public Guid ItemUid { get; private set; }

  public int Quantity { get; private set; }

  public InventoryEntity(TrainerEntity trainer, ItemEntity item, InventoryItemAdded @event) : this(trainer, item)
  {
    Add(@event);
  }
  public InventoryEntity(TrainerEntity trainer, ItemEntity item, InventoryItemUpdated @event) : this(trainer, item)
  {
    Update(@event);
  }
  private InventoryEntity(TrainerEntity trainer, ItemEntity item)
  {
    Trainer = trainer;
    TrainerId = trainer.TrainerId;
    TrainerUid = trainer.Id;

    Item = item;
    ItemId = item.ItemId;
    ItemUid = item.Id;
  }

  private InventoryEntity()
  {
  }

  public void Add(InventoryItemAdded @event)
  {
    Quantity += @event.Quantity;
  }

  public void Remove(InventoryItemRemoved @event)
  {
    Quantity -= @event.Quantity;
  }

  public void Update(InventoryItemUpdated @event)
  {
    Quantity = @event.Quantity;
  }

  public override bool Equals(object? obj) => obj is InventoryEntity entity && entity.TrainerUid == TrainerUid && entity.ItemId == ItemId;
  public override int GetHashCode() => HashCode.Combine(TrainerUid, ItemId);
  public override string ToString() => $"{GetType()} (TrainerUid={TrainerUid}, ItemId={ItemId})";
}
