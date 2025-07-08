using CsvHelper.Configuration;

namespace PokeGame.Seeding.Game.Payloads;

internal class PokeBallPayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int Price { get; set; }

  public double CatchMultiplier { get; set; }
  public bool Heal { get; set; }
  public int BaseFriendship { get; set; }
  public int FriendshipMultiplier { get; set; }

  public string? Sprite { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is PokeBallPayload pokeball && pokeball.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<PokeBallPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);

      Map(x => x.CatchMultiplier).Index(5).Default(0.0);
      Map(x => x.Heal).Index(6).Default(false);
      Map(x => x.BaseFriendship).Index(7).Default(0);
      Map(x => x.FriendshipMultiplier).Index(8).Default(0);

      Map(x => x.Sprite).Index(9);

      Map(x => x.Url).Index(10);
      Map(x => x.Notes).Index(11);
    }
  }
}
