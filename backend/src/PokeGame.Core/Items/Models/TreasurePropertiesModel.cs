using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record TreasurePropertiesModel : ITreasureProperties
{
  [JsonConstructor]
  public TreasurePropertiesModel()
  {
  }

  public TreasurePropertiesModel(ITreasureProperties _) : this()
  {
  }
}
