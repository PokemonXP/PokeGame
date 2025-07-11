using Krakenar.Contracts.Search;

namespace PokeGame.Core.Items.Models;

public record SearchItemsPayload : SearchPayload
{
  public ItemCategory? Category { get; set; }

  public new List<ItemSortOption> Sort { get; set; } = [];
}
