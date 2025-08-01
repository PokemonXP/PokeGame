namespace PokeGame.Core.Battles.Models;

public record StatisticChangesPayload
{
  public int Attack { get; set; }
  public int Defense { get; set; }
  public int SpecialAttack { get; set; }
  public int SpecialDefense { get; set; }
  public int Speed { get; set; }
  public int Accuracy { get; set; }
  public int Evasion { get; set; }
  public int Critical { get; set; }
}
