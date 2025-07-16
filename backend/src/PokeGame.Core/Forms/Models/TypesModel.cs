namespace PokeGame.Core.Forms.Models;

public record TypesModel : ITypes
{
  public PokemonType Primary { get; set; }
  public PokemonType? Secondary { get; set; }

  public TypesModel()
  {
  }

  public TypesModel(PokemonType primary, PokemonType? secondary = null)
  {
    Primary = primary;
    Secondary = secondary;
  }

  public TypesModel(ITypes types) : this(types.Primary, types.Secondary)
  {
  }
}
