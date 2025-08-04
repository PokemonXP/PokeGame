namespace PokeGame.ETL.Models;

internal record FlavorText
{
  [JsonPropertyName("flavor_text")]
  public string Text { get; set; } = string.Empty;

  [JsonPropertyName("language")]
  public NamedResource Language { get; set; } = new();

  [JsonPropertyName("version_group")]
  public VersionGroup Version { get; set; } = new();
}
