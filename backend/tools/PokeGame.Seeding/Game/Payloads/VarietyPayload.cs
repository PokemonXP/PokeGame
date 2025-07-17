using CsvHelper.Configuration;

namespace PokeGame.Seeding.Game.Payloads;

internal class VarietyPayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Species { get; set; } = string.Empty;
  public bool IsDefault { get; set; }

  public bool CanChangeForm { get; set; }
  public int? GenderRatio { get; set; }

  public string Genus { get; set; } = string.Empty;
  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is VarietyPayload variety && variety.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<VarietyPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(3).Default(string.Empty);
      Map(x => x.DisplayName).Index(4);
      Map(x => x.Description).Index(5);

      Map(x => x.Species).Index(1).Default(string.Empty);
      Map(x => x.IsDefault).Index(2).Default(false);

      Map(x => x.CanChangeForm).Index(6).Default(false);
      Map(x => x.GenderRatio).Index(7);

      Map(x => x.Genus).Index(8).Default(string.Empty);
      Map(x => x.Url).Index(9);
      Map(x => x.Notes).Index(10);
    }
  }
}
