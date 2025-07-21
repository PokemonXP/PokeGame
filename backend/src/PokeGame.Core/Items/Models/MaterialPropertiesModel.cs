using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record MaterialPropertiesModel : IMaterialProperties
{
  [JsonConstructor]
  public MaterialPropertiesModel()
  {
  }

  public MaterialPropertiesModel(IMaterialProperties _) : this()
  {
  }
}
