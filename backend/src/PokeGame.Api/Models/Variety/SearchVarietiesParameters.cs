using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Api.Models.Variety;

public record SearchVarietiesParameters : SearchParameters
{
  [FromQuery(Name = "species")]
  public Guid? SpeciesId { get; set; }

  public virtual SearchVarietiesPayload ToPayload()
  {
    SearchVarietiesPayload payload = new()
    {
      SpeciesId = SpeciesId
    };
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
