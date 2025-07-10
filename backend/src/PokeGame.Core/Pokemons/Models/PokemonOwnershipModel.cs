using PokeGame.Core.Items.Models;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Core.Pokemons.Models;

public record PokemonOwnershipModel
{
  public OwnershipKind Kind { get; set; }
  public TrainerModel Trainer { get; set; } = new();
  public ItemModel PokeBall { get; set; } = new();
  public int Level { get; set; }
  public string Location { get; set; } = string.Empty;
  public DateTime MetOn { get; set; }
  public string? Description { get; set; }
}
