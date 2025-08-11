namespace PokeGame.ETL.Models;

internal record VarietySprites
{
  [JsonPropertyName("other")]
  public OtherSprites Other { get; set; } = new();
}

internal record OtherSprites
{
  [JsonPropertyName("home")]
  public HomeSprites Home { get; set; } = new();
}

internal record HomeSprites
{
  [JsonPropertyName("front_default")]
  public string Default { get; set; } = string.Empty;

  [JsonPropertyName("front_shiny")]
  public string Shiny { get; set; } = string.Empty;

  [JsonPropertyName("front_female")]
  public string? Alternative { get; set; }

  [JsonPropertyName("front_shiny_female")]
  public string? AlternativeShiny { get; set; }
}
