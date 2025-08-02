namespace PokeGame.Core.Battles.Models;

public record BattleMoveTargetPayload
{
  public Guid TargetId { get; set; } // TASK: [POKEGAME-296](https://logitar.atlassian.net/browse/POKEGAME-296)
  public DamagePayload? Damage { get; set; }
  public StatusConditionPayload? Status { get; set; }
  public StatisticChangesPayload Statistics { get; set; } = new();
}
