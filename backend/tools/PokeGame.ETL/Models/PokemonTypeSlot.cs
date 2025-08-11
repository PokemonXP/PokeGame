namespace PokeGame.ETL.Models;

internal record PokemonTypeSlot
{
  [JsonPropertyName("slot")]
  public int Slot { get; set; }

  [JsonPropertyName("type")]
  public NamedResource Type { get; set; } = new();
}
