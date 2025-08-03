namespace PokeGame.ETL.Models;

internal record FlavorText
{
  [JsonPropertyName("flavor_text")]
  public string Text { get; set; } = string.Empty;

  [JsonPropertyName("language")]
  public Language Language { get; set; } = new();

  // TODO(fpion): version_group
}
