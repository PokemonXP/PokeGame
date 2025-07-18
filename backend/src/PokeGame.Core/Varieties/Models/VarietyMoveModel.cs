using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Varieties.Models;

public record VarietyMoveModel
{
  public MoveModel Move { get; set; } = new();
  public int Level { get; set; }
}
