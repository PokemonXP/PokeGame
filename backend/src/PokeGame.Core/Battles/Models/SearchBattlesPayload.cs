using Krakenar.Contracts.Search;

namespace PokeGame.Core.Battles.Models;

public record SearchBattlesPayload : SearchPayload
{
  public BattleKind? Kind { get; set; }
  public BattleStatus? Status { get; set; }
  public BattleResolution? Resolution { get; set; }
  public Guid? TrainerId { get; set; }

  public new List<BattleSortOption> Sort { get; set; } = [];
}
