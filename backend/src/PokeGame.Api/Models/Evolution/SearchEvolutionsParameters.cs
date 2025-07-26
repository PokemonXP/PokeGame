using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Evolutions.Models;

namespace PokeGame.Api.Models.Evolution;

public record SearchEvolutionsParameters : SearchParameters
{
  [FromQuery(Name = "source")]
  public Guid? SourceId { get; set; }

  [FromQuery(Name = "target")]
  public Guid? TargetId { get; set; }

  [FromQuery(Name = "trigger")]
  public EvolutionTrigger? Trigger { get; set; }

  public virtual SearchEvolutionsPayload ToPayload()
  {
    SearchEvolutionsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out EvolutionSort field))
      {
        payload.Sort.Add(new EvolutionSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
