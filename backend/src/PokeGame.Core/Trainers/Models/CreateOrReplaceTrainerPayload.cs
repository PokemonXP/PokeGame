namespace PokeGame.Core.Trainers.Models;

public record CreateOrReplaceTrainerPayload
{
  public string License { get; set; } = string.Empty;

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public TrainerGender Gender { get; set; }
  public int Money { get; set; }

  public string? Sprite { get; set; }
  public string? Url { get; set; }
  public string? Notes { get; set; }
}
