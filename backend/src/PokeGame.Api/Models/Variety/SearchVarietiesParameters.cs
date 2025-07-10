using Krakenar.Contracts.Search;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Api.Models.Variety;

public record SearchVarietiesParameters : SearchParameters
{
  public virtual SearchVarietiesPayload ToPayload()
  {
    SearchVarietiesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out VarietySort field))
      {
        payload.Sort.Add(new VarietySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
