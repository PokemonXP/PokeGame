using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;
using PokeGame.Core.Items;

namespace PokeGame.Core.Inventory;

public class InsufficientInventoryQuantityException : BadRequestException
{
  private const string ErrorMessage = "The inventory does not have sufficient quantities of the specified item.";

  public Guid TrainerId
  {
    get => (Guid)Data[nameof(TrainerId)]!;
    private set => Data[nameof(TrainerId)] = value;
  }
  public Guid ItemId
  {
    get => (Guid)Data[nameof(ItemId)]!;
    private set => Data[nameof(ItemId)] = value;
  }
  public int RequiredQuantity
  {
    get => (int)Data[nameof(RequiredQuantity)]!;
    private set => Data[nameof(RequiredQuantity)] = value;
  }
  public int AvailableQuantity
  {
    get => (int)Data[nameof(AvailableQuantity)]!;
    private set => Data[nameof(AvailableQuantity)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(TrainerId)] = TrainerId;
      error.Data[nameof(ItemId)] = ItemId;
      error.Data[nameof(RequiredQuantity)] = RequiredQuantity;
      error.Data[nameof(AvailableQuantity)] = AvailableQuantity;
      return error;
    }
  }

  public InsufficientInventoryQuantityException(TrainerInventory inventory, Item item, int requiredQuantity)
    : this(inventory, item.Id, requiredQuantity)
  {
  }
  public InsufficientInventoryQuantityException(TrainerInventory inventory, ItemId itemId, int requiredQuantity)
    : base(BuildMessage(inventory, itemId, requiredQuantity))
  {
    TrainerId = inventory.TrainerId.ToGuid();
    ItemId = itemId.ToGuid();
    RequiredQuantity = requiredQuantity;
    AvailableQuantity = inventory.GetQuantity(itemId);
  }

  private static string BuildMessage(TrainerInventory inventory, ItemId itemId, int requiredQuantity) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TrainerId), inventory.TrainerId.ToGuid())
    .AddData(nameof(ItemId), itemId.ToGuid())
    .AddData(nameof(RequiredQuantity), requiredQuantity)
    .AddData(nameof(AvailableQuantity), inventory.GetQuantity(itemId))
    .Build();
}
