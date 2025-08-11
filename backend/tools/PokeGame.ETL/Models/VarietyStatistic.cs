namespace PokeGame.ETL.Models;

internal record VarietyStatistic
{
  [JsonPropertyName("base_stat")]
  public byte BaseValue { get; set; }

  [JsonPropertyName("effort")]
  public int EffortValue { get; set; }

  [JsonPropertyName("stat")]
  public NamedResource Statistic { get; set; } = new();
}
