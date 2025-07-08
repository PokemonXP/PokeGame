namespace PokeGame.Core.Items.Models;

public record PowerPointRestoreModel
{
  public int Value { get; set; }
  public bool IsPercentage { get; set; }

  public bool AllMoves { get; set; }

  public PowerPointRestoreModel()
  {
  }

  public PowerPointRestoreModel(int value, bool isPercentage = false, bool allMoves = false)
  {
    Value = value;
    IsPercentage = isPercentage;

    AllMoves = allMoves;
  }
}
