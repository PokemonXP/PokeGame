using PokeGame.Core.Moves;

namespace PokeGame.Core.Items.Properties;

public record TechnicalMachineProperties : ItemProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.TechnicalMachine;

  public MoveId MoveId { get; }

  public TechnicalMachineProperties(MoveId moveId)
  {
    MoveId = moveId;
  }
}
