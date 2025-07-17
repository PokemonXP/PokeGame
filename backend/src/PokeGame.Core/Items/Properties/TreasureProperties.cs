namespace PokeGame.Core.Items.Properties;

public record TreasureProperties : ItemProperties, ITreasureProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Treasure;
}
