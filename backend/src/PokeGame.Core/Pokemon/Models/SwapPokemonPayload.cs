namespace PokeGame.Core.Pokemon.Models;

public record SwapPokemonPayload
{
  public string Source { get; set; } = string.Empty;
  public string Destination { get; set; } = string.Empty;
}
