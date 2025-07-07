namespace PokeGame.Core.Forms.Models;

public record SpritesModel
{
  public string Default { get; set; } = string.Empty;
  public string DefaultShiny { get; set; } = string.Empty;
  public string? Alternative { get; set; }
  public string? AlternativeShiny { get; set; }
}
