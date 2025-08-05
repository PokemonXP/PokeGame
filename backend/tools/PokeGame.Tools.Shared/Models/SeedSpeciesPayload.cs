using CsvHelper.Configuration;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Models;

namespace PokeGame.Tools.Shared.Models;

public record SeedSpeciesPayload : CreateOrReplaceSpeciesPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedSpeciesPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Name("id").Default(Guid.Empty);

      Map(x => x.Number).Index(1).Name("number").Default(0);
      Map(x => x.Category).Index(2).Name("category").Default(default(PokemonCategory));

      Map(x => x.UniqueName).Index(3).Name("unique_name").Default(string.Empty);
      Map(x => x.DisplayName).Index(4).Name("display_name");

      Map(x => x.BaseFriendship).Index(5).Name("base_friendship").Default(0);
      Map(x => x.CatchRate).Index(6).Name("catch_rate").Default(0);
      Map(x => x.GrowthRate).Index(7).Name("growth_rate").Default(default(GrowthRate));

      Map(x => x.EggCycles).Index(8).Name("egg_cycles").Default(0);
      References<EggGroupsMap>(x => x.EggGroups);

      Map(x => x.Url).Name("url").Index(11);
      Map(x => x.Notes).Name("notes").Index(12);
    }
  }

  private class EggGroupsMap : ClassMap<EggGroupsModel>
  {
    public EggGroupsMap()
    {
      Map(x => x.Primary).Index(9).Name("egg_group_primary").Default(default(EggGroup));
      Map(x => x.Secondary).Index(10).Name("egg_group_secondary");
    }
  }
}
