namespace PokeGame.Core.Items.Properties;

public abstract record ItemProperties
{
  [JsonInclude]
  public abstract ItemCategory Category { get; }
}
