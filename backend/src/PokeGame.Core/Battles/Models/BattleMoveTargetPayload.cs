namespace PokeGame.Core.Battles.Models;

public record BattleMoveTargetPayload
{
  public Guid TargetId { get; set; } // TODO(fpion): ID or UniqueName
  public DamagePayload? Damage { get; set; }
  public StatusConditionPayload? Status { get; set; }
  public StatisticChangesPayload Statistics { get; set; } = new();
}
