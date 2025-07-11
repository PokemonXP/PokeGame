using CsvHelper.Configuration;
using PokeGame.Core.Species;

namespace PokeGame.Seeding.Game.Payloads;

internal class SpeciesPayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public int Number { get; set; }
  public PokemonCategory Category { get; set; }

  public int BaseFriendship { get; set; }
  public int CatchRate { get; set; }
  public GrowthRate GrowthRate { get; set; }

  public List<RegionalNumberPayload> RegionalNumbers { get; set; } = [];

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is SpeciesPayload species && species.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<SpeciesPayload>
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

      Map(x => x.RegionalNumbers).Convert(args =>
      {
        string? values = args.Row.GetField(8);
        return string.IsNullOrWhiteSpace(values) ? [] : values.Split('|').Select(value =>
        {
          string[] values = value.Split(':');
          RegionalNumberPayload regionalNumber = new();
          if (values.Length == 2)
          {
            regionalNumber.Region = values.First();
            if (int.TryParse(values.Last(), out int number))
            {
              regionalNumber.Number = number;
            }
          }
          return regionalNumber;
        }).ToList();
      });

      Map(x => x.Url).Index(9);
      Map(x => x.Notes).Index(10);
    }
  }
}
