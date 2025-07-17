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

  // TODO(fpion): Egg(Groups and Cycles)

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public List<RegionalNumberPayload> RegionalNumbers { get; set; } = [];
}
