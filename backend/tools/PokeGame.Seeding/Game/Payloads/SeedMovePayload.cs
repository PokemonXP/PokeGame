using CsvHelper.Configuration;
using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedMovePayload : CreateOrReplaceMovePayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedMovePayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.Type).Index(1).Default(default(PokemonType));
      Map(x => x.Category).Index(2).Default(default(MoveCategory));

      Map(x => x.UniqueName).Index(3).Default(string.Empty);
      Map(x => x.DisplayName).Index(4);
      Map(x => x.Description).Index(5);

      Map(x => x.Accuracy).Index(6);
      Map(x => x.Power).Index(7);
      Map(x => x.PowerPoints).Index(8);

      Map(x => x.Url).Index(9);
      Map(x => x.Notes).Index(10);
    }
  }
}
