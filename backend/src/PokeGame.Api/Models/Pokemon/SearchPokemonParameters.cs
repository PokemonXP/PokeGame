using Krakenar.Contracts.Search;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Api.Models.Pokemon;

public record SearchPokemonParameters : SearchParameters
{
  public virtual SearchPokemonPayload ToPayload()
  {
    SearchPokemonPayload payload = new();
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
