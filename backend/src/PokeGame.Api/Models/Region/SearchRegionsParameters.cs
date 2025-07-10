using Krakenar.Contracts.Search;
using PokeGame.Core.Regions.Models;

namespace PokeGame.Api.Models.Region;

public record SearchRegionsParameters : SearchParameters
{
  public virtual SearchRegionsPayload ToPayload()
  {
    SearchRegionsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out RegionSort field))
      {
        payload.Sort.Add(new RegionSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
