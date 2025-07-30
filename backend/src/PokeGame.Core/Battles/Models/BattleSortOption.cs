using Krakenar.Contracts.Search;

namespace PokeGame.Core.Battles.Models;

public record BattleSortOption : SortOption
{
  public new BattleSort Field
  {
    get => Enum.Parse<BattleSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public BattleSortOption() : this(BattleSort.Name)
  {
  }

  public BattleSortOption(BattleSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
