namespace PokeGame.Core.Species.Models;

public record CreateOrReplaceSpeciesPayload
{
  public int Number { get; set; }
  public PokemonCategory Category { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public byte BaseFriendship { get; set; }
  public byte CatchRate { get; set; }
  public GrowthRate GrowthRate { get; set; }

  public byte EggCycles { get; set; }
  public EggGroupsModel EggGroups { get; set; } = new();

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public List<RegionalNumberPayload> RegionalNumbers { get; set; } = [];
}
