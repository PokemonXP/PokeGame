namespace PokeGame.Core.Varieties.Models;

public record CreateOrReplaceVarietyPayload
{
  public string Species { get; set; } = string.Empty;
  public bool IsDefault { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public string? Genus { get; set; }
  public string? Description { get; set; }

  public int? GenderRatio { get; set; }

  public bool CanChangeForm { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public List<VarietyMovePayload> Moves { get; set; } = [];
}
