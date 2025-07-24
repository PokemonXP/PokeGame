using PokeGame.Core.Trainers.Models;

namespace PokeGame.Api.Models.Game;

public record TrainerSummary
{
  public string Name { get; set; }
  public string? Sprite { get; set; } // TODO(fpion: License instead of sprite

  public TrainerSummary() : this(string.Empty)
  {
  }

  public TrainerSummary(string name)
  {
    Name = name;
  }

  public TrainerSummary(TrainerModel trainer)
  {
    Name = trainer.DisplayName ?? trainer.UniqueName;
    Sprite = trainer.Sprite;
  }
}
