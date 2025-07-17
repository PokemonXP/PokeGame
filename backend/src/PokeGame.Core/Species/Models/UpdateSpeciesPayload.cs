using Krakenar.Contracts;

namespace PokeGame.Core.Species.Models;

public record UpdateSpeciesPayload
{
  public string? UniqueName { get; set; } = string.Empty;
  public Change<string>? DisplayName { get; set; }

  public byte? BaseFriendship { get; set; }
  public byte? CatchRate { get; set; }
  public GrowthRate? GrowthRate { get; set; }

  // TODO(fpion): Egg(Groups and Cycles)

  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }

  public List<RegionalNumberPayload> RegionalNumbers { get; set; } = [];
}
