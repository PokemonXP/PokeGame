using PokeGame.Core;

namespace PokeGame.Seeding.Game.Payloads;

public record InflictedStatusPayload
{
  public StatusCondition Condition { get; set; }
  public int Chance { get; set; }
}
