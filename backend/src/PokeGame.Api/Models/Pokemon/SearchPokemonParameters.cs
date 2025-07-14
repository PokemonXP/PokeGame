using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Api.Models.Pokemon;

public record SearchPokemonParameters : SearchParameters
{
  [FromQuery(Name = "trainer")]
  public Guid? TrainerId { get; set; }

  public virtual SearchPokemonPayload ToPayload()
  {
    SearchPokemonPayload payload = new()
    {
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
