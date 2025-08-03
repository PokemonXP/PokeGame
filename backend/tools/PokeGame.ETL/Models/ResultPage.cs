namespace PokeGame.ETL.Models;

internal record ResultPage
{
  [JsonPropertyName("count")]
  public int Count { get; set; }

  [JsonPropertyName("next")]
  public string? Next { get; set; }

  [JsonPropertyName("previous")]
  public string? Previous { get; set; }

  [JsonPropertyName("results")]
  public List<SearchResult> Results { get; set; } = [];
}
