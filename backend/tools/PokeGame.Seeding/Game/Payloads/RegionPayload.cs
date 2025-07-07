using CsvHelper.Configuration;

namespace PokeGame.Seeding.Game.Payloads;

internal class RegionPayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is RegionPayload region && region.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<RegionPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Url).Index(4);
      Map(x => x.Notes).Index(5);
    }
  }
}
