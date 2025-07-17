using Krakenar.Contracts;

namespace PokeGame.Core.Regions.Models;

public record UpdateRegionPayload
{
  public string? UniqueName { get; set; } = string.Empty;
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }
}
