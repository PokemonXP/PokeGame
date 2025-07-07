using Krakenar.Contracts.Search;

namespace PokeGame.Core.Trainers.Models;

public record TrainerSortOption : SortOption
{
  public new TrainerSort Field
  {
    get => Enum.Parse<TrainerSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public TrainerSortOption() : this(TrainerSort.DisplayName)
  {
  }

  public TrainerSortOption(TrainerSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
