using Krakenar.Contracts;

namespace PokeGame.Core.Species.Models;

public record UpdateSpeciesPayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }

  public byte? BaseFriendship { get; set; }
  public byte? CatchRate { get; set; }
  public GrowthRate? GrowthRate { get; set; }

  public byte? EggCycles { get; set; }
  public EggGroupsModel? EggGroups { get; set; }

  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }

  public List<RegionalNumberPayload> RegionalNumbers { get; set; } = [];
}
