using PokeGame.Core.Trainers.Models;

namespace PokeGame.Api.Models.Game;

public record TrainerSummary
{
  public string License { get; set; }
  public string Name { get; set; }

  public TrainerSummary() : this(string.Empty, string.Empty)
  {
  }

  public TrainerSummary(string license, string name)
  {
    License = license;
    Name = name;
  }

  public TrainerSummary(TrainerModel trainer)
  {
    License = trainer.License;
    Name = trainer.DisplayName ?? trainer.UniqueName;
  }
}
