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

  // TODO(fpion): DisplayName (species.names)

  // TODO(fpion): Genus (species.genera)

  // TODO(fpion): Description (species.flavor_text_entries)

  // TODO(fpion): GenderRatio (species.gender_rate)

  // TODO(fpion): CanChangeForm (species.forms_switchable)

  /*
   * Unmapped fields:
   * abilities
   * base_experience
   * cries
   * forms
   * game_indices
   * height
   * held_items
   * location_area_encounters
   * moves
   * order
   * past_abilities
   * past_types
   * sprites
   * stats
   * types
   * weight
   */
}
