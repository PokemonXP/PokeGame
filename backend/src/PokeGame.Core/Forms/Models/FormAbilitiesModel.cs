using PokeGame.Core.Abilities.Models;

namespace PokeGame.Core.Forms.Models;

public record FormAbilitiesModel
{
  public AbilityModel Primary { get; set; } = new();
  public AbilityModel? Secondary { get; set; }
  public AbilityModel? Hidden { get; set; }
}
