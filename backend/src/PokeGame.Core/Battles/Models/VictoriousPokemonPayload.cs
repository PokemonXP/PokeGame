namespace PokeGame.Core.Battles.Models;

public record VictoriousPokemonPayload
{
  public string Pokemon { get; set; } = string.Empty;
  public int Experience { get; set; }
}
