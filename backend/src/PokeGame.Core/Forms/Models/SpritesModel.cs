namespace PokeGame.Core.Forms.Models;

public record SpritesModel
{
  public string Default { get; set; }
  public string Shiny { get; set; }
  public string? Alternative { get; set; }
  public string? AlternativeShiny { get; set; }

  public SpritesModel() : this(string.Empty, string.Empty)
  {
  }

  public SpritesModel(string @default, string shiny, string? alternative = null, string? alternativeShiny = null)
  {
    Default = @default;
    Shiny = shiny;
    Alternative = alternative;
    AlternativeShiny = alternativeShiny;
  }
}
