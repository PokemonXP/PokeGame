using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Moves.Models;

public class MoveModel : AggregateModel
{
  public PokemonType Type { get; set; }
  public MoveCategory Category { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int Accuracy { get; set; }
  public int Power { get; set; }
  public int PowerPoints { get; set; }

  public InflictedStatusModel? Status { get; set; }
  public List<string> VolatileConditions { get; set; } = [];

  public StatisticChangesModel StatisticChanges { get; set; } = new();

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
