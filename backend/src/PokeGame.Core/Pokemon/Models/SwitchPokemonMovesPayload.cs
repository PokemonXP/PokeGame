namespace PokeGame.Core.Pokemon.Models;

public record SwitchPokemonMovesPayload
{
  public int Source { get; set; }
  public int Destination { get; set; }
}
