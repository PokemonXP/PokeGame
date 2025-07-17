using CsvHelper.Configuration;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedSpeciesPayload : CreateOrReplaceSpeciesPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedSpeciesPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(3).Default(string.Empty);
      Map(x => x.DisplayName).Index(4);

      Map(x => x.Number).Index(1).Default(0);
      Map(x => x.Category).Index(2).Default(default(PokemonCategory));

      Map(x => x.BaseFriendship).Index(5).Default(0);
      Map(x => x.CatchRate).Index(6).Default(0);
      Map(x => x.GrowthRate).Index(7).Default(default(GrowthRate));

      //Map(x => x.RegionalNumbers).Convert(args =>
      //{
      //  string? values = args.Row.GetField(8);
      //  return string.IsNullOrWhiteSpace(values) ? [] : values.Split('|').Select(value =>
      //  {
      //    string[] values = value.Split(':');
      //    RegionalNumberPayload regionalNumber = new();
      //    if (values.Length == 2)
      //    {
      //      regionalNumber.Region = values.First();
      //      if (int.TryParse(values.Last(), out int number))
      //      {
      //        regionalNumber.Number = number;
      //      }
      //    }
      //    return regionalNumber;
      //  }).ToList();
      //}); // TODO(fpion): implement

      Map(x => x.Url).Index(9);
      Map(x => x.Notes).Index(10);
    }
  }
}
