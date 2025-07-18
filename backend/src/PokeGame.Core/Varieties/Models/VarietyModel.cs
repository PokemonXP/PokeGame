using PokeGame.Core.Forms.Models;
using PokeGame.Core.Species.Models;
using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Varieties.Models;

public class VarietyModel : AggregateModel
{
  public SpeciesModel Species { get; set; } = new();
  public bool IsDefault { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public string? Genus { get; set; }
  public string? Description { get; set; }

  public int? GenderRatio { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public bool CanChangeForm { get; set; }
  public List<FormModel> Forms { get; set; } = [];

  public List<VarietyMoveModel> Moves { get; set; } = [];

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
