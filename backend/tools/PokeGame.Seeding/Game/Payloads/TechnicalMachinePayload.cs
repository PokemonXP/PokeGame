using CsvHelper.Configuration;

namespace PokeGame.Seeding.Game.Payloads;

internal class TechnicalMachinePayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int Price { get; set; }

  public string Move { get; set; } = string.Empty;

  public string? Sprite { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is TechnicalMachinePayload TechnicalMachine && TechnicalMachine.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<TechnicalMachinePayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);

      Map(x => x.Move).Index(5).Default(string.Empty);

      Map(x => x.Sprite).Index(6);

      Map(x => x.Url).Index(7);
      Map(x => x.Notes).Index(8);
    }
  }
}
