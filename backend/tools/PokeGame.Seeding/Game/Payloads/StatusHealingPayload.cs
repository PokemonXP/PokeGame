using PokeGame.Core;

namespace PokeGame.Seeding.Game.Payloads;

public record StatusHealingPayload
{
  public StatusCondition? Condition { get; set; }
  public bool All { get; set; }

  public StatusHealingPayload()
  {
  }

  public StatusHealingPayload(StatusCondition? condition, bool all = false)
  {
    Condition = condition;
    All = all;
  }
}
