using PokeGame.Core.Items.Models;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Pokemon.Models;

public record PokemonMoveModel
{
  public MoveModel Move { get; set; } = new();

  public int? Position { get; set; }

  public PowerPointsModel PowerPoints { get; set; } = new();
  public bool IsMastered { get; set; }

  public int Level { get; set; }
  public MoveLearningMethod Method { get; set; }
  public ItemModel? Item { get; set; }

  public string? Notes { get; set; }
}
