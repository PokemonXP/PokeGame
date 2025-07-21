using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Items;

public class UnexpectedItemCategoryException : BadRequestException
{
  private const string ErrorMessage = "The specified item category was not expected.";

  public Guid ItemId
  {
    get => (Guid)Data[nameof(ItemId)]!;
    private set => Data[nameof(ItemId)] = value;
  }
  public ItemCategory ExpectedCategory
  {
    get => (ItemCategory)Data[nameof(ExpectedCategory)]!;
    private set => Data[nameof(ExpectedCategory)] = value;
  }
  public ItemCategory ActualCategory
  {
    get => (ItemCategory)Data[nameof(ActualCategory)]!;
    private set => Data[nameof(ActualCategory)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(ItemId)] = ItemId;
      error.Data[nameof(ExpectedCategory)] = ExpectedCategory;
      error.Data[nameof(ActualCategory)] = ActualCategory;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UnexpectedItemCategoryException(ItemCategory expectedCategory, Item item, string propertyName)
    : base(BuildMessage(expectedCategory, item, propertyName))
  {
    ItemId = item.Id.ToGuid();
    ExpectedCategory = expectedCategory;
    ActualCategory = item.Category;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ItemCategory expectedCategory, Item item, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ItemId), item.Id.ToGuid())
    .AddData(nameof(ExpectedCategory), expectedCategory)
    .AddData(nameof(ActualCategory), item.Category)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
