using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record MedicinePropertiesModel : IMedicineProperties
{
  public bool IsHerbal { get; set; }

  public int Healing { get; set; }
  public bool IsHealingPercentage { get; set; }
  public bool Revives { get; set; }

  public StatusCondition? StatusCondition { get; set; }
  public bool AllConditions { get; set; }

  public int PowerPoints { get; set; }
  public bool IsPowerPointPercentage { get; set; }
  public bool RestoreAllMoves { get; set; }
}
