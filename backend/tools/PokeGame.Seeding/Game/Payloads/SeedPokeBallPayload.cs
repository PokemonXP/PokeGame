using CsvHelper.Configuration;
using PokeGame.Core.Items.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedPokeBallPayload : CreateOrReplaceItemPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedPokeBallPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);

      References<PokeBallPropertiesMap>(x => x.PokeBall);

      Map(x => x.Sprite).Index(9);
      Map(x => x.Url).Index(10);
      Map(x => x.Notes).Index(11);
    }
  }

  private class PokeBallPropertiesMap : ClassMap<PokeBallPropertiesModel>
  {
    public PokeBallPropertiesMap()
    {
      Map(x => x.CatchMultiplier).Index(5).Default(1.0);
      Map(x => x.Heal).Index(6).Default(false);
      Map(x => x.BaseFriendship).Index(7).Default(0);
      Map(x => x.FriendshipMultiplier).Index(8).Default(1.0);
    }
  }
}
