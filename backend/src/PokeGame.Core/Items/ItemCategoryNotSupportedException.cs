namespace PokeGame.Core.Items;

public class ItemCategoryNotSupportedException : NotSupportedException
{
  public ItemCategory Category
  {
    get => (ItemCategory)Data[nameof(Category)]!;
    private set => Data[nameof(Category)] = value;
  }

  public ItemCategoryNotSupportedException(ItemCategory category)
    : base($"The item category '{category}' is not supported.")
  {
    Category = category;
  }
}
