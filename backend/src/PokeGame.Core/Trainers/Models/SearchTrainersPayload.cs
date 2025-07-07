using Krakenar.Contracts.Search;

namespace PokeGame.Core.Trainers.Models;

public record SearchTrainersPayload : SearchPayload
{
  public TrainerGender? Gender { get; set; }
  public Guid? UserId { get; set; }

  public new List<TrainerSortOption> Sort { get; set; } = [];
}
