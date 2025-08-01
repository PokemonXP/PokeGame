namespace PokeGame.Core.Battles.Models;

public record UseBattleMovePayload
{
  public string Attacker { get; set; } = string.Empty;
  public string Move { get; set; } = string.Empty;
  public byte PowerPointCost { get; set; }
  public int StaminaCost { get; set; }

  public List<BattleMoveTargetPayload> Targets { get; set; } = [];
}
