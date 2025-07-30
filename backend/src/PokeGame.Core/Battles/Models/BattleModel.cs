using PokeGame.Core.Trainers.Models;
using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Battles.Models;

public class BattleModel : AggregateModel
{
  public BattleKind Kind { get; set; }
  public BattleStatus Status { get; set; }

  public string Name { get; set; } = string.Empty;
  public string Location { get; set; } = string.Empty;
  public string? Url { get; set; }
  public string? Notes { get; set; }

  public List<TrainerModel> Champions { get; set; } = [];
  // TODO(fpion): Opponents

  public override string ToString() => $"{Name} | {base.ToString()}";
}
