using PokeGame.Core.Varieties.Models;
using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Species.Models;

public class SpeciesModel : AggregateModel
{
  public int Number { get; set; }
  public PokemonCategory Category { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public int BaseFriendship { get; set; }
  public int CatchRate { get; set; }
  public GrowthRate GrowthRate { get; set; }

  public List<RegionalNumberModel> RegionalNumbers { get; set; } = [];
  public List<VarietyModel> Varieties { get; set; } = [];

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
