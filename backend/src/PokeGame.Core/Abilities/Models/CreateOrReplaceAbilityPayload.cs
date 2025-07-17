namespace PokeGame.Core.Abilities.Models;

public record CreateOrReplaceAbilityPayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }
}
