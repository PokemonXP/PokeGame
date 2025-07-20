using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record BerryPropertiesModel : IBerryProperties
{
  [JsonConstructor]
  public BerryPropertiesModel()
  {
  }

  public BerryPropertiesModel(IBerryProperties _) : this()
  {
  }
}
