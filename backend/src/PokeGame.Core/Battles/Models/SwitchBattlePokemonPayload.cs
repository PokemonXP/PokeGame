namespace PokeGame.Core.Battles.Models;

public record SwitchBattlePokemonPayload
{
  public string Active { get; set; } = string.Empty;
  public string Inactive { get; set; } = string.Empty;
}
