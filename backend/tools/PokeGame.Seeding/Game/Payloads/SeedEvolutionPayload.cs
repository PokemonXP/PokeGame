using CsvHelper.Configuration;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Evolutions.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedEvolutionPayload : CreateOrReplaceEvolutionPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedEvolutionPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.Source).Index(1).Default(string.Empty);
      Map(x => x.Target).Index(2).Default(string.Empty);

      Map(x => x.Trigger).Index(3).Default(default(EvolutionTrigger));
      Map(x => x.Item).Index(4);

      Map(x => x.Level).Index(5).Default(0);
      Map(x => x.Friendship).Index(6).Default(false);
      Map(x => x.Gender).Index(7);
      Map(x => x.HeldItem).Index(8);
      Map(x => x.KnownMove).Index(9);
      Map(x => x.Location).Index(10);
      Map(x => x.TimeOfDay).Index(11);
    }
  }
}
