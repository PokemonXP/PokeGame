using PokeGame.Core.Varieties.Models;
using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Forms.Models;

public class FormModel : AggregateModel
{
  public VarietyModel Variety { get; set; } = new();
  public bool IsDefault { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public bool IsBattleOnly { get; set; }
  public bool IsMega { get; set; }

  public int Height { get; set; }
  public int Weight { get; set; }

  public FormTypesModel Types { get; set; } = new();
  public FormAbilitiesModel Abilities { get; set; } = new();
  public BaseStatisticsModel BaseStatistics { get; set; } = new();
  public YieldModel Yield { get; set; } = new();
  public SpritesModel Sprites { get; set; } = new();

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
