namespace PokeGame.Core.Items.Models;

public record HealingModel
{
  public int Value { get; set; }
  public bool IsPercentage { get; set; }

  public bool Revive { get; set; }

  public HealingModel()
  {
  }

  public HealingModel(int value, bool isPercentage = false, bool revive = false)
  {
    Value = value;
    IsPercentage = isPercentage;

    Revive = revive;
  }
}
