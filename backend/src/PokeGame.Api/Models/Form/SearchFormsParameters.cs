using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Api.Models.Form;

public record SearchFormsParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public PokemonType? Type { get; set; }

  [FromQuery(Name = "ability")]
  public Guid? AbilityId { get; set; }

  public virtual SearchFormsPayload ToPayload()
  {
    SearchFormsPayload payload = new()
    {
      Type = Type,
      AbilityId = AbilityId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out FormSort field))
      {
        payload.Sort.Add(new FormSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
