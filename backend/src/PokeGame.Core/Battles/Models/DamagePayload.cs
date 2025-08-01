namespace PokeGame.Core.Battles.Models;

public record DamagePayload
{
  public int Value { get; set; }
  public bool IsPercentage { get; set; }
  public bool IsHealing { get; set; }
}
