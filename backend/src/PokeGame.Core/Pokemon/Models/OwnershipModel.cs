using PokeGame.Core.Items.Models;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Core.Pokemon.Models;

public record OwnershipModel
{
  public TrainerModel OriginalTrainer { get; set; } = new();
  public TrainerModel CurrentTrainer { get; set; } = new();
  public ItemModel PokeBall { get; set; } = new();

  public OwnershipKind Kind { get; set; }
  public int Level { get; set; }
  public string Location { get; set; } = string.Empty;
  public DateTime MetOn { get; set; }
  public string? Description { get; set; }

  public int Position { get; set; }
  public int? Box { get; set; }
}
