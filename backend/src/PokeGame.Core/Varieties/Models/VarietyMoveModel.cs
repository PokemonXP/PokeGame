using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Varieties.Models;

public record VarietyMoveModel
{
  public MoveModel Move { get; set; }
  public int Level { get; set; }

  public VarietyMoveModel() : this(new MoveModel(), level: 0)
  {
  }

  public VarietyMoveModel(MoveModel move, int level)
  {
    Move = move;
    Level = level;
  }
}
