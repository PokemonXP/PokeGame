using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Moves.Models;

public class MoveModel : AggregateModel
{
  public PokemonType Type { get; set; }
  public MoveCategory Category { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public byte? Accuracy { get; set; }
  public byte? Power { get; set; }
  public byte PowerPoints { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
