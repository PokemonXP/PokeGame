namespace PokeGame.Core.Items.Properties;

public interface IBerryProperties
{
  int Healing { get; }
  bool IsHealingPercentage { get; }

  StatusCondition? StatusCondition { get; }
  bool AllConditions { get; }
  bool CureConfusion { get; }

  int PowerPoints { get; }

  int Attack { get; }
  int Defense { get; }
  int SpecialAttack { get; }
  int SpecialDefense { get; }
  int Speed { get; }
  int Accuracy { get; }
  int Evasion { get; }
  int Critical { get; }

  PokemonStatistic? LowerEffortValues { get; }
  bool RaiseFriendship { get; }
}
