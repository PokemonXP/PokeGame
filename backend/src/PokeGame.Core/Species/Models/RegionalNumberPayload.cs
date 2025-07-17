namespace PokeGame.Core.Species.Models;

public record RegionalNumberPayload
{
  public string Region { get; set; } = string.Empty;
  public int Number { get; set; }
}
