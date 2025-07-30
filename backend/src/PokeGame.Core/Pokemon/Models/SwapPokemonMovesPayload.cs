namespace PokeGame.Core.Pokemon.Models;

public record SwapPokemonMovesPayload
{
  public int Source { get; set; }
  public int Destination { get; set; }
}
