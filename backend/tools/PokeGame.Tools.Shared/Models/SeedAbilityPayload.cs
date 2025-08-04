using CsvHelper.Configuration;
using PokeGame.Core.Abilities.Models;

namespace PokeGame.Tools.Shared.Models;

public record SeedAbilityPayload : CreateOrReplaceAbilityPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedAbilityPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Name("id").Default(Guid.Empty);

      Map(x => x.UniqueName).Name("unique_name").Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Name("display_name").Index(2);
      Map(x => x.Description).Name("description").Index(3);

      Map(x => x.Url).Name("url").Index(4);
      Map(x => x.Notes).Name("notes").Index(5);
    }
  }
}
