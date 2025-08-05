namespace PokeGame.ETL.Models;

internal record Genus
{
  [JsonPropertyName("genus")]
  public string Value { get; set; } = string.Empty;

  [JsonPropertyName("language")]
  public NamedResource Language { get; set; } = new();
}
