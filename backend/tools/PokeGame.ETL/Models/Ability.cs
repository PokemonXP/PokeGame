namespace PokeGame.ETL.Models;

internal record Ability
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("names")]
  public List<LocalizedName> DisplayNames { get; set; } = [];

  [JsonPropertyName("flavor_text_entries")]
  public List<FlavorText> Descriptions { get; set; } = [];

  /*
   * Unmapped fields:
   * effect_changes
   * effect_entries
   * generation
   * is_main_series
   * pokemon
   */
}
