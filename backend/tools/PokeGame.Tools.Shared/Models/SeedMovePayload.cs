using CsvHelper.Configuration;
using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Tools.Shared.Models;

public record SeedMovePayload : CreateOrReplaceMovePayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedMovePayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Name("id").Default(Guid.Empty);

      Map(x => x.Type).Index(1).Name("type").Default(default(PokemonType));
      Map(x => x.Category).Index(2).Name("category").Default(default(MoveCategory));

      Map(x => x.UniqueName).Index(3).Name("unique_name").Default(string.Empty);
      Map(x => x.DisplayName).Index(4).Name("display_name");
      Map(x => x.Description).Index(5).Name("description");

      Map(x => x.Accuracy).Index(6).Name("accuracy");
      Map(x => x.Power).Index(7).Name("power");
      Map(x => x.PowerPoints).Index(8).Name("power_points");

      Map(x => x.Url).Index(9).Name("url");
      Map(x => x.Notes).Index(10).Name("notes");
    }
  }
}
