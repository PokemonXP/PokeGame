namespace PokeGame.Core.Battles.Models;

public record UseBattleMovePayload
{
  public Guid AttackerId { get; set; } // TODO(fpion): ID or UniqueName
  public string Move { get; set; } = string.Empty;
  public byte PowerPointCost { get; set; }
  public int StaminaCost { get; set; }

  public List<BattleMoveTargetPayload> Targets { get; set; } = [];
}
