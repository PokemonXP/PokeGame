using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;

namespace PokeGame.Api.Models.Item;

public record SearchItemsParameters : SearchParameters
{
  [FromQuery(Name = "category")]
  public ItemCategory? Category { get; set; }

  public virtual SearchItemsPayload ToPayload()
  {
    SearchItemsPayload payload = new()
    {
      Category = Category
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ItemSort field))
      {
        payload.Sort.Add(new ItemSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
