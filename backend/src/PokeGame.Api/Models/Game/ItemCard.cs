using PokeGame.Core.Items.Models;

namespace PokeGame.Api.Models.Game;

public record ItemCard
{
  public string Name { get; set; }
  public string? Sprite { get; set; }

  public ItemCard() : this(string.Empty)
  {
  }

  public ItemCard(string name)
  {
    Name = name;
  }

  public ItemCard(ItemModel item)
  {
    Name = item.DisplayName ?? item.UniqueName;
    Sprite = item.Sprite;
  }
}
