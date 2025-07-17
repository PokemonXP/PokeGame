using Krakenar.Contracts;

namespace PokeGame.Core.Trainers.Models;

public record UpdateTrainerPayload
{
  public string? UniqueName { get; set; } = string.Empty;
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public TrainerGender? Gender { get; set; }
  public int? Money { get; set; }

  public Change<Guid?>? UserId { get; set; }

  public Change<string>? Sprite { get; set; }
  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }
}
