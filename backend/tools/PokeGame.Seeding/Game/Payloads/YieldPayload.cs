namespace PokeGame.Seeding.Game.Payloads;

internal record YieldPayload
{
  public int Experience { get; set; }

  public int HP { get; set; }
  public int Attack { get; set; }
  public int Defense { get; set; }
  public int SpecialAttack { get; set; }
  public int SpecialDefense { get; set; }
  public int Speed { get; set; }
}
