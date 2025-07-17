using CsvHelper.Configuration;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedTrainerPayload : CreateOrReplaceTrainerPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedTrainerPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.License).Index(5).Default(string.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Gender).Index(4).Default(default(TrainerGender));
      Map(x => x.Money).Index(6).Default(0);

      Map(x => x.UserId).Index(8);

      Map(x => x.Sprite).Index(7);
      Map(x => x.Url).Index(9);
      Map(x => x.Notes).Index(10);
    }
  }
}
