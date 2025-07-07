using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Cms.Models.Move;

public record SearchMovesParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public PokemonType? Type { get; set; }

  [FromQuery(Name = "category")]
  public MoveCategory? Category { get; set; }

  [FromQuery(Name = "status")]
  public StatusCondition? StatusCondition { get; set; }

  public virtual SearchMovesPayload ToPayload()
  {
    SearchMovesPayload payload = new()
    {
      Type = Type,
      Category = Category,
      StatusCondition = StatusCondition
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out MoveSort field))
      {
        payload.Sort.Add(new MoveSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
