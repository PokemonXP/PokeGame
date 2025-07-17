using Krakenar.Contracts;

namespace PokeGame.Core.Forms.Models;

public record UpdateFormPayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public bool? IsBattleOnly { get; set; }
  public bool? IsMega { get; set; }

  public int? Height { get; set; }
  public int? Weight { get; set; }

  public FormTypesModel? Types { get; set; }
  public FormAbilitiesPayload? Abilities { get; set; }
  public BaseStatisticsModel? BaseStatistics { get; set; }
  public YieldModel? Yield { get; set; }
  public SpritesModel? Sprites { get; set; }

  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }
}
