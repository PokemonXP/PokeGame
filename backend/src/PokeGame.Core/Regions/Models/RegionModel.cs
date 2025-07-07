using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Regions.Models;

public class RegionModel : AggregateModel
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
