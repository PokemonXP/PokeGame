using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record KeyItemPropertiesModel : IKeyItemProperties
{
  [JsonConstructor]
  public KeyItemPropertiesModel()
  {
  }

  public KeyItemPropertiesModel(IKeyItemProperties _) : this()
  {
  }
}
