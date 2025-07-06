namespace PokeGame.Seeding.Game.Payloads;

internal record PowerPointRestorePayload
{
  public int Value { get; set; }
  public bool IsPercentage { get; set; }

  public bool AllMoves { get; set; }

  public PowerPointRestorePayload()
  {
  }

  public PowerPointRestorePayload(int value, bool isPercentage = false, bool allMoves = false)
  {
    Value = value;
    IsPercentage = isPercentage;

    AllMoves = allMoves;
  }
}
