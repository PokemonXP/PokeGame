namespace PokeGame.ETL.Models;

internal class Species
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("is_baby")]
  public bool IsBaby { get; set; }

  [JsonPropertyName("is_legendary")]
  public bool IsLegendary { get; set; }

  [JsonPropertyName("is_mythical")]
  public bool IsMythical { get; set; }

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("names")]
  public List<LocalizedName> DisplayNames { get; set; } = [];

  [JsonPropertyName("genera")]
  public List<Genus> Genera { get; set; } = [];

  [JsonPropertyName("base_happiness")]
  public byte BaseFriendship { get; set; }

  [JsonPropertyName("capture_rate")]
  public byte CatchRate { get; set; }

  [JsonPropertyName("growth_rate")]
  public NamedResource GrowthRate { get; set; } = new();

  [JsonPropertyName("hatch_counter")]
  public byte EggCycles { get; set; }

  [JsonPropertyName("egg_groups")]
  public List<NamedResource> EggGroups { get; set; } = [];

  [JsonPropertyName("pokedex_numbers")]
  public List<PokedexNumber> PokedexNumbers { get; set; } = [];

  [JsonPropertyName("gender_rate")]
  public int GenderRatio { get; set; }

  [JsonPropertyName("forms_switchable")]
  public bool CanChangeForm { get; set; }

  /*
   * Unmapped fields:
   * color
   * evolution_chain
   * evolves_from_species
   * flavor_text_entries
   * form_descriptions
   * generation
   * habitat
   * has_gender_differences
   * order
   * pal_park_encounters
   * shape
   * varieties
   */
}
