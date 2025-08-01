namespace PokeGame.Core.Battles.Models;

public record StatusConditionPayload
{
  public StatusCondition? Condition { get; set; }
  public bool RemoveCondition { get; set; }
  public bool AllConditions { get; set; }
}
