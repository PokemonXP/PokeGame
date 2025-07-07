using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Trainers.Models;

public class TrainerModel : AggregateModel
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public TrainerGender Gender { get; set; }
  public string License { get; set; } = string.Empty;
  public int Money { get; set; }

  public Guid? UserId { get; set; }

  public string? Sprite { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
