namespace PokeGame.Core.Battles.Models;

public record BattleMoveTargetPayload
{
  public string Target { get; set; } = string.Empty;
  public DamagePayload? Damage { get; set; }
  public StatusConditionPayload? Status { get; set; }
  public StatisticChangesPayload Statistics { get; set; } = new();
}
