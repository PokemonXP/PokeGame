namespace PokeGame.Core.Battles.Models;

public record UseBattleMovePayload
{
  public Guid AttackerId { get; set; } // TASK: [POKEGAME-296](https://logitar.atlassian.net/browse/POKEGAME-296)
  public string Move { get; set; } = string.Empty;
  public byte PowerPointCost { get; set; }
  public int StaminaCost { get; set; }

  public List<BattleMoveTargetPayload> Targets { get; set; } = [];
}
