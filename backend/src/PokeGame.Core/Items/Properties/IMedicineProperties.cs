namespace PokeGame.Core.Items.Properties;

public interface IMedicineProperties
{
  bool IsHerbal { get; }

  int Healing { get; }
  bool IsHealingPercentage { get; }
  bool Revives { get; }

  StatusCondition? StatusCondition { get; }
  bool AllConditions { get; }

  int PowerPoints { get; }
  bool IsPowerPointPercentage { get; }
  bool RestoreAllMoves { get; }
}
