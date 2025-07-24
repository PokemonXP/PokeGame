using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Api.Models.Game;

public class TrainerSheet
{
  public Guid Id { get; set; }

  public string License { get; set; }
  public string Name { get; set; }
  public TrainerGender Gender { get; set; }
  public int Money { get; set; }

  public string? Sprite { get; set; }

  public TrainerSheet() : this(string.Empty, string.Empty)
  {
  }

  public TrainerSheet(string license, string name)
  {
    License = license;
    Name = name;
  }

  public TrainerSheet(TrainerModel trainer)
  {
    Id = trainer.Id;

    License = trainer.License;
    Name = trainer.DisplayName ?? trainer.UniqueName;
    Gender = trainer.Gender;
    Money = trainer.Money;

    Sprite = trainer.Sprite;
  }

  public override bool Equals(object? obj) => obj is TrainerSheet trainer && trainer.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Name} (Id={Id})";
}
