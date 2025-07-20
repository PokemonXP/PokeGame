namespace PokeGame.Core.Items.Properties;

public record BerryProperties : ItemProperties, IBerryProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Berry;
}
