namespace PokeGame.Core.Moves.Models;

public record InflictedStatusModel
{
  public StatusCondition Condition { get; set; }
  public int Chance { get; set; }

  public InflictedStatusModel()
  {
  }

  public InflictedStatusModel(StatusCondition condition, int chance)
  {
    Condition = condition;
    Chance = chance;
  }
}
