namespace PokeGame.Seeding.Game.Payloads;

internal record StatisticChangesPayload
{
  public int Attack { get; set; }
  public int Defense { get; set; }
  public int SpecialAttack { get; set; }
  public int SpecialDefense { get; set; }
  public int Speed { get; set; }
  public int Accuracy { get; set; }
  public int Evasion { get; set; }
}
