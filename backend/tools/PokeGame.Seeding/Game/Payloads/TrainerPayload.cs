using CsvHelper.Configuration;
using PokeGame.Core.Trainers;

namespace PokeGame.Seeding.Game.Payloads;

internal class TrainerPayload
{
  public Guid Id { get; set; }

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

  public override bool Equals(object? obj) => obj is TrainerPayload trainer && trainer.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<TrainerPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Gender).Index(4).Default(default(TrainerGender));
      Map(x => x.License).Index(5).Default(string.Empty);
      Map(x => x.Money).Index(6).Default(0);

      Map(x => x.Sprite).Index(7);

      Map(x => x.UserId).Index(8);

      Map(x => x.Url).Index(9);
      Map(x => x.Notes).Index(10);
    }
  }
}
