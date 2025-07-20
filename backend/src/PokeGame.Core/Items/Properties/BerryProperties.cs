namespace PokeGame.Core.Items.Properties;

public record BerryProperties : ItemProperties, IBerryProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Berry;

  [JsonConstructor]
  public BerryProperties()
  {
  }

  public BerryProperties(IBerryProperties _) : this()
  {
  }
}
