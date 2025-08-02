namespace PokeGame.Core.Battles.Models;

public record BattleStatisticModel
{
  public int Unmodified { get; set; }
  public int Modified { get; set; }
  public int Stages { get; set; }

  public BattleStatisticModel()
  {
  }

  public BattleStatisticModel(int value, int stages)
  {
    Unmodified = value;
    Modified = value;
    Stages = stages;

    if (stages > 0)
    {
      Modified = value * Math.Min(2 + stages, 8) / 2;
    }
    else if (stages < 0)
    {
      Modified = value * 2 / Math.Min(2 - stages, 8);
    }
  }
}
