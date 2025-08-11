namespace PokeGame.ETL.Models;

internal record Variety
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("species")]
  public NamedResource Species { get; set; } = new();

  [JsonPropertyName("is_default")]
  public bool IsDefault { get; set; }

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("height")]
  public int Height { get; set; }

  [JsonPropertyName("weight")]
  public int Weight { get; set; }

  [JsonPropertyName("abilities")]
  public List<PokemonAbilitySlot> Abilities { get; set; } = [];

  [JsonPropertyName("base_experience")]
  public int BaseExperience { get; set; }

  [JsonPropertyName("stats")]
  public List<VarietyStatistic> Statistics { get; set; } = [];

  [JsonPropertyName("sprites")]
  public VarietySprites Sprites { get; set; } = new();

  /*
   * Unmapped fields:
   * cries
   * forms
   * game_indices
   * held_items
   * location_area_encounters
   * moves
   * order
   * past_abilities
   * past_types
   * types
   */
}
