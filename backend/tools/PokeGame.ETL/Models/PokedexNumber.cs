namespace PokeGame.ETL.Models;

internal record PokedexNumber
{
  [JsonPropertyName("entry_number")]
  public int Number { get; set; }

  [JsonPropertyName("pokedex")]
  public NamedResource Pokedex { get; set; } = new();
}
