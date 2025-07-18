using FluentValidation;
using PokeGame.Core.Forms.Validators;

namespace PokeGame.Core.Forms;

public record FormTypes : IFormTypes
{
  public PokemonType Primary { get; }
  public PokemonType? Secondary { get; }

  public FormTypes()
  {
  }

  [JsonConstructor]
  public FormTypes(PokemonType primary, PokemonType? secondary = null)
  {
    Primary = primary;
    Secondary = secondary;
    new FormTypesValidator().ValidateAndThrow(this);
  }

  public FormTypes(IFormTypes types) : this(types.Primary, types.Secondary)
  {
  }
}
