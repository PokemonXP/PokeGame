namespace PokeGame.ETL.Models;

internal record Form
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("pokemon")]
  public NamedResource Variety { get; set; } = new();

  [JsonPropertyName("is_default")]
  public bool IsDefault { get; set; }

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("names")]
  public List<LocalizedName> DisplayNames { get; set; } = [];

  [JsonPropertyName("is_battle_only")]
  public bool IsBattleOnly { get; set; }

  [JsonPropertyName("is_mega")]
  public bool IsMega { get; set; }

  [JsonPropertyName("types")]
  public List<PokemonTypeSlot> Types { get; set; } = [];

  /*
   * Unmapped fields:
   * form_name
   * form_names
   * form_order
   * order
   * sprites
   * version_group
   */
}
