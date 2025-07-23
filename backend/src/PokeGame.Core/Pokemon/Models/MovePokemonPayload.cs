namespace PokeGame.Core.Pokemon.Models;

public record MovePokemonPayload
{
  public int Position { get; set; }
  public int Box { get; set; }
}
