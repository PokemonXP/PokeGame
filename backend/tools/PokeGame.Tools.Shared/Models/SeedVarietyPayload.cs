using CsvHelper.Configuration;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Tools.Shared.Models;

public record SeedVarietyPayload : CreateOrReplaceVarietyPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedVarietyPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Name("id").Default(Guid.Empty);

      Map(x => x.Species).Index(1).Name("species").Default(string.Empty);
      Map(x => x.IsDefault).Index(2).Name("is_default").Default(false);

      Map(x => x.UniqueName).Index(3).Name("unique_name").Default(string.Empty);
      Map(x => x.DisplayName).Index(4).Name("display_name");

      Map(x => x.Genus).Index(5).Name("genus").Default(string.Empty);
      Map(x => x.Description).Index(6).Name("description");

      Map(x => x.GenderRatio).Index(7).Name("gender_ratio");

      Map(x => x.CanChangeForm).Index(8).Name("can_change_form").Default(false);

      Map(x => x.Url).Index(9).Name("url");
      Map(x => x.Notes).Index(10).Name("notes");
    }
  }
}
