namespace PokeGame.ETL.Models;

internal record PokemonAbilitySlot
{
  [JsonPropertyName("ability")]
  public NamedResource Ability { get; set; } = new();

  [JsonPropertyName("is_hidden")]
  public bool IsHidden { get; set; }

  [JsonPropertyName("slot")]
  public int Slot { get; set; }
}
