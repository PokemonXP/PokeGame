namespace PokeGame.Core.Items.Properties;

public record MaterialProperties : ItemProperties, IMaterialProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Material;
}
