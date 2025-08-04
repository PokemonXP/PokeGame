namespace PokeGame.ETL.Models;

internal record Move
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("type")]
  public NamedResource Type { get; set; } = new();

  [JsonPropertyName("damage_class")]
  public NamedResource Category { get; set; } = new();

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("names")]
  public List<LocalizedName> DisplayNames { get; set; } = [];

  [JsonPropertyName("flavor_text_entries")]
  public List<FlavorText> Descriptions { get; set; } = [];

  [JsonPropertyName("accuracy")]
  public byte? Accuracy { get; set; }

  [JsonPropertyName("power")]
  public byte? Power { get; set; }

  [JsonPropertyName("pp")]
  public byte? PowerPoints { get; set; }

  /*
   * Unmapped fields:
   * contest_combos
   * contest_effect
   * contest_type
   * effect_chance
   * effect_changes
   * effect_entries
   * generation
   * learned_by_pokemon
   * machines
   * meta
   * past_values
   * priority
   * stat_changes
   * super_contest_effect
   * target
   */
}
