using Krakenar.Contracts.Search;

namespace PokeGame.Core.Varieties.Models;

public record VarietySortOption : SortOption
{
  public new VarietySort Field
  {
    get => Enum.Parse<VarietySort>(base.Field);
    set => base.Field = value.ToString();
  }

  public VarietySortOption() : this(VarietySort.DisplayName)
  {
  }

  public VarietySortOption(VarietySort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
