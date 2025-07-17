using CsvHelper.Configuration;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedVarietyPayload : CreateOrReplaceVarietyPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedVarietyPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(3).Default(string.Empty);
      Map(x => x.DisplayName).Index(4);

      Map(x => x.Genus).Index(8).Default(string.Empty);
      Map(x => x.Description).Index(5);

      Map(x => x.Species).Index(1).Default(string.Empty);
      Map(x => x.IsDefault).Index(2).Default(false);

      Map(x => x.GenderRatio).Index(7);

      Map(x => x.CanChangeForm).Index(6).Default(false);

      Map(x => x.Url).Index(9);
      Map(x => x.Notes).Index(10);
    }
  }
}
