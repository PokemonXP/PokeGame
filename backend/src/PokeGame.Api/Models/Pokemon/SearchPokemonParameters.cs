using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Api.Models.Pokemon;

public record SearchPokemonParameters : SearchParameters
{
  [FromQuery(Name = "species")]
  public Guid? SpeciesId { get; set; }

  [FromQuery(Name = "item")]
  public Guid? HeldItemId { get; set; }

  [FromQuery(Name = "trainer")]
  public Guid? TrainerId { get; set; }

  public virtual SearchPokemonPayload ToPayload()
  {
    SearchPokemonPayload payload = new()
    {
      SpeciesId = SpeciesId,
      HeldItemId = HeldItemId,
      TrainerId = TrainerId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out PokemonSort field))
      {
        payload.Sort.Add(new PokemonSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
