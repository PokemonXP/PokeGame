using Krakenar.Contracts.Search;

namespace PokeGame.Core.Moves.Models;

public record MoveSortOption : SortOption
{
  public new MoveSort Field
  {
    get => Enum.Parse<MoveSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public MoveSortOption() : this(MoveSort.DisplayName)
  {
  }

  public MoveSortOption(MoveSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
