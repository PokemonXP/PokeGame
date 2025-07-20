namespace PokeGame.Core.Items.Properties;

public record PicnicItemProperties : ItemProperties, IPicnicItemProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.PicnicItem;
}
