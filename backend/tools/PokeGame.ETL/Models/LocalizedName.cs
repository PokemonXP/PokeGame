namespace PokeGame.ETL.Models;

internal record LocalizedName
{
  [JsonPropertyName("language")]
  public Language Language { get; set; } = new();

  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;
}
