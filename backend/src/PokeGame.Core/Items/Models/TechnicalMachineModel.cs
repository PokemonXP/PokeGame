using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Items.Models;

public record TechnicalMachineModel
{
  public MoveModel Move { get; set; } = new();

  public TechnicalMachineModel()
  {
  }

  public TechnicalMachineModel(MoveModel move)
  {
    Move = move;
  }
}
