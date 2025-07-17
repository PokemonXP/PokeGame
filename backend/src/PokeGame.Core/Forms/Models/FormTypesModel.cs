namespace PokeGame.Core.Forms.Models;

public record FormTypesModel : IFormTypes
{
  public PokemonType Primary { get; set; }
  public PokemonType? Secondary { get; set; }

  public FormTypesModel()
  {
  }

  public FormTypesModel(PokemonType primary, PokemonType? secondary = null)
  {
    Primary = primary;
    Secondary = secondary;
  }

  public FormTypesModel(IFormTypes types) : this(types.Primary, types.Secondary)
  {
  }
}
