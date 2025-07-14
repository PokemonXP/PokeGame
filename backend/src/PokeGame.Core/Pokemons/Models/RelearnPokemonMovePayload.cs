namespace PokeGame.Core.Pokemons.Models;

public record RelearnPokemonMovePayload
{
  public string Move { get; set; } = string.Empty;
  public int Position { get; set; }
}
