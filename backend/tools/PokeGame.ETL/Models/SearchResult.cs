namespace PokeGame.ETL.Models;

internal record SearchResult
{
  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("url")]
  public string Url { get; set; } = string.Empty;
}
