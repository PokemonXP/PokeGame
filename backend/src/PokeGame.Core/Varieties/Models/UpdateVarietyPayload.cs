using Krakenar.Contracts;

namespace PokeGame.Core.Varieties.Models;

public record UpdateVarietyPayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }

  public Change<string>? Genus { get; set; }
  public Change<string>? Description { get; set; }

  public Change<int?>? GenderRatio { get; set; }

  public bool? CanChangeForm { get; set; }

  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }

  public List<VarietyMovePayload> Moves { get; set; } = [];
}
