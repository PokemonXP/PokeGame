namespace PokeGame.Core.Items.Properties;

public record OtherItemProperties : ItemProperties, IOtherItemProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.OtherItem;
}
