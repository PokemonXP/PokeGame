using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Items.Models;

public record TechnicalMachinePropertiesModel
{
  public MoveModel Move { get; set; }

  public TechnicalMachinePropertiesModel() : this(new MoveModel())
  {
  }

  public TechnicalMachinePropertiesModel(MoveModel move)
  {
    Move = move;
  }
}
