using PokeGame.Core.Items.Models;

namespace PokeGame.Api.Models.Game;

public record HeldItem
{
  public string Name { get; set; }
  public string? Sprite { get; set; }

  public HeldItem() : this(string.Empty)
  {
  }

  public HeldItem(string name)
  {
    Name = name;
  }

  public HeldItem(ItemModel item)
  {
    Name = item.DisplayName ?? item.UniqueName;
    Sprite = item.Sprite;
  }
}
