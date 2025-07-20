namespace PokeGame.Core.Items.Properties;

public record TechnicalMachineMaterialProperties : ItemProperties, ITechnicalMachineMaterialProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.TechnicalMachineMaterial;
}
