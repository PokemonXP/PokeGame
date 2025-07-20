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

  public MedicinePropertiesModel()
  {
  }

  [JsonConstructor]
  public MedicinePropertiesModel(
    bool isHerbal,
    int healing = 0,
    bool isHealingPercentage = false,
    bool revives = false,
    StatusCondition? statusCondition = null,
    bool allConditions = false,
    int powerPoints = 0,
    bool isPowerPointPercentage = false,
    bool restoreAllMoves = false)
  {
    IsHerbal = isHerbal;

    Healing = healing;
    IsHealingPercentage = isHealingPercentage;
    Revives = revives;

    StatusCondition = statusCondition;
    AllConditions = allConditions;

    PowerPoints = powerPoints;
    IsPowerPointPercentage = isPowerPointPercentage;
    RestoreAllMoves = restoreAllMoves;
  }

  public MedicinePropertiesModel(IMedicineProperties medicine) : this(
    medicine.IsHerbal,
    medicine.Healing,
    medicine.IsHealingPercentage,
    medicine.Revives,
    medicine.StatusCondition,
    medicine.AllConditions,
    medicine.PowerPoints,
    medicine.IsPowerPointPercentage,
    medicine.RestoreAllMoves)
  {
  }
}
