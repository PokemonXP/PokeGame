using PokeGame.Core.Items.Models;

namespace PokeGame.Api.Models.Game;

public record ItemSummary
{
  public string Name { get; set; }
  public string? Description { get; set; }
  public string? Sprite { get; set; }

  public ItemSummary() : this(string.Empty)
  {
  }

  public ItemSummary(string name)
  {
    Name = name;
  }

  public ItemSummary(ItemModel item)
  {
    Name = item.DisplayName ?? item.UniqueName;
    Description = item.Description;
    Sprite = item.Sprite;
  }
}
