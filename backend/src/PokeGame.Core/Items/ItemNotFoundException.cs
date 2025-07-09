using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Items;

public class ItemNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified item was not found.";

  public string Item
  {
    get => (string)Data[nameof(Item)]!;
    private set => Data[nameof(Item)] = value;
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
      error.Data[nameof(Item)] = Item;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ItemNotFoundException(string item, string propertyName) : base(BuildMessage(item, propertyName))
  {
    Item = item;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string item, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Item), item)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
