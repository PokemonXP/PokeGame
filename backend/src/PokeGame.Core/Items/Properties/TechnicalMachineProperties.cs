using PokeGame.Core.Moves;

namespace PokeGame.Core.Items.Properties;

public record TechnicalMachineProperties : ItemProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.TechnicalMachine;

  public MoveId MoveId { get; }

  [JsonConstructor]
  public TechnicalMachineProperties(MoveId moveId)
  {
    MoveId = moveId;
  }

  public TechnicalMachineProperties(Move move) : this(move.Id)
  {
  }
}
