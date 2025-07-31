using Krakenar.Contracts;

namespace PokeGame.Core.Battles.Models;

public record UpdateBattlePayload
{
  public string? Name { get; set; }
  public string? Location { get; set; }
  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }
}
