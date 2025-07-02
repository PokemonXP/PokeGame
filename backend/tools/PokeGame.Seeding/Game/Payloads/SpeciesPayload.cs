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
}
