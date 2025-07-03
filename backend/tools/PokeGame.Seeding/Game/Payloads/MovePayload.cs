using PokeGame.Core;
using PokeGame.Core.Moves;

namespace PokeGame.Seeding.Game.Payloads;

internal class MovePayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public PokemonType Type { get; set; }
  public MoveCategory Category { get; set; }

  public int? Accuracy { get; set; }
  public int? Power { get; set; }
  public int PowerPoints { get; set; }

  public InflictedStatusPayload? InflictedStatus { get; set; }
  public List<VolatileCondition> VolatileConditions { get; set; } = [];
  public List<StatisticChangePayload> StatisticChanges { get; set; } = [];

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is MovePayload move && move.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";
}
