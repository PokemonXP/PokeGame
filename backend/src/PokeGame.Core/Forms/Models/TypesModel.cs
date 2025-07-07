namespace PokeGame.Core.Forms.Models;

public record TypesModel
{
  public PokemonType Primary { get; set; }
  public PokemonType? Secondary { get; set; }
}
