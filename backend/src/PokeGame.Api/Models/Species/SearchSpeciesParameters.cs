using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Models;

namespace PokeGame.Api.Models.Species;

public record SearchSpeciesParameters : SearchParameters
{
  [FromQuery(Name = "category")]
  public PokemonCategory? Category { get; set; }

  [FromQuery(Name = "egg")]
  public EggGroup? EggGroup { get; set; }

  [FromQuery(Name = "growth")]
  public GrowthRate? GrowthRate { get; set; }

  [FromQuery(Name = "region")]
  public Guid? RegionId { get; set; }

  public virtual SearchSpeciesPayload ToPayload()
  {
    SearchSpeciesPayload payload = new()
    {
      Category = Category,
      EggGroup = EggGroup,
      GrowthRate = GrowthRate,
      RegionId = RegionId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out SpeciesSort field))
      {
        payload.Sort.Add(new SpeciesSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
