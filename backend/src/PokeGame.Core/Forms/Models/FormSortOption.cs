using Krakenar.Contracts.Search;

namespace PokeGame.Core.Forms.Models;

public record FormSortOption : SortOption
{
  public new FormSort Field
  {
    get => Enum.Parse<FormSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public FormSortOption() : this(FormSort.DisplayName)
  {
  }

  public FormSortOption(FormSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
