using CsvHelper.Configuration;
using PokeGame.Core.Items.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedItemPayload : CreateOrReplaceItemPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedItemPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);
      //Map(x => x.Category).Index(5); // TODO(fpion): implement

      Map(x => x.Sprite).Index(6);

      Map(x => x.Url).Index(7);
      Map(x => x.Notes).Index(8);
    }
  }
}
