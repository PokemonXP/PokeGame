namespace PokeGame.Core.Battles.Models;

public record CreateBattlePayload
{
  public Guid? Id { get; set; }

  public BattleKind Kind { get; set; }

  public string Name { get; set; } = string.Empty;
  public string Location { get; set; } = string.Empty;
  public string? Url { get; set; }
  public string? Notes { get; set; }

  public List<string> Champions { get; set; } = [];
  public List<string> Opponents { get; set; } = [];
}
