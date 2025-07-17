namespace PokeGame.Core.Forms.Models;

public record CreateOrReplaceFormPayload
{
  public string Variety { get; set; } = string.Empty;
  public bool IsDefault { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public bool IsBattleOnly { get; set; }
  public bool IsMega { get; set; }

  public int Height { get; set; }
  public int Weight { get; set; }

  public FormTypesModel Types { get; set; } = new();
  public FormAbilitiesPayload Abilities { get; set; } = new();
  public BaseStatisticsModel BaseStatistics { get; set; } = new();
  public YieldModel Yield { get; set; } = new();
  public SpritesModel Sprites { get; set; } = new();

  public string? Url { get; set; }
  public string? Notes { get; set; }
}
