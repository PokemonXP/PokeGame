using CsvHelper.Configuration;
using PokeGame.Core.Abilities.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedAbilityPayload : CreateOrReplaceAbilityPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedAbilityPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Url).Index(4);
      Map(x => x.Notes).Index(5);
    }
  }
}
