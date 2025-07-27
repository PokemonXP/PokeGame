using Krakenar.Contracts.Search;

namespace PokeGame.Core.Evolutions.Models;

public record EvolutionSortOption : SortOption
{
  public new EvolutionSort Field
  {
    get => Enum.Parse<EvolutionSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public EvolutionSortOption() : this(EvolutionSort.Level)
  {
  }

  public EvolutionSortOption(EvolutionSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
