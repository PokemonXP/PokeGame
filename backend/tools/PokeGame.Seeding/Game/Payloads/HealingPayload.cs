namespace PokeGame.Seeding.Game.Payloads;

internal record HealingPayload
{
  public int Value { get; set; }
  public bool IsPercentage { get; set; }

  public bool Revive { get; set; }

  public HealingPayload()
  {
  }

  public HealingPayload(int value, bool isPercentage = false, bool revive = false)
  {
    Value = value;
    IsPercentage = isPercentage;

    Revive = revive;
  }
}
