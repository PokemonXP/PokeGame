using Krakenar.Contracts.Search;

namespace PokeGame.Core.Items.Models;

public record ItemSortOption : SortOption
{
  public new ItemSort Field
  {
    get => Enum.Parse<ItemSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ItemSortOption() : this(ItemSort.DisplayName)
  {
  }

  public ItemSortOption(ItemSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
