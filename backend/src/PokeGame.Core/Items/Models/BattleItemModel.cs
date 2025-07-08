namespace PokeGame.Core.Items.Models;

public record BattleItemModel
{
  public int AttackChange { get; set; }
  public int DefenseChange { get; set; }
  public int SpecialAttackChange { get; set; }
  public int SpecialDefenseChange { get; set; }
  public int SpeedChange { get; set; }
  public int AccuracyChange { get; set; }
  public int EvasionChange { get; set; }
  public int CriticalChange { get; set; }
  public int GuardTurns { get; set; }

  public BattleItemModel()
  {
  }

  public BattleItemModel(
    int attackChange,
    int defenseChange,
    int specialAttackChange,
    int specialDefenseChange,
    int speedChange,
    int accuracyChange,
    int evasionChange,
    int criticalChange,
    int guardTurns)
  {
    AttackChange = attackChange;
    DefenseChange = defenseChange;
    SpecialAttackChange = specialAttackChange;
    SpecialDefenseChange = specialDefenseChange;
    SpeedChange = speedChange;
    AccuracyChange = accuracyChange;
    EvasionChange = evasionChange;
    CriticalChange = criticalChange;
    GuardTurns = guardTurns;
  }
}
