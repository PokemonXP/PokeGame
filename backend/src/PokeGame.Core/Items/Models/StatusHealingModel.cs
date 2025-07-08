namespace PokeGame.Core.Items.Models;

public record StatusHealingModel
{
  public StatusCondition? Condition { get; set; }
  public bool All { get; set; }

  public StatusHealingModel()
  {
  }

  public StatusHealingModel(StatusCondition? condition, bool all = false)
  {
    Condition = condition;
    All = all;
  }
}
