using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Api.Models.Move;

public record SearchMovesParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public PokemonType? Type { get; set; }

  [FromQuery(Name = "category")]
  public MoveCategory? Category { get; set; }

  public virtual SearchMovesPayload ToPayload()
  {
    SearchMovesPayload payload = new()
    {
      Type = Type,
      Category = Category
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
