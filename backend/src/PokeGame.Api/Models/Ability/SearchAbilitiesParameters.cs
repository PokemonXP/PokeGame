using Krakenar.Contracts.Search;
using PokeGame.Core.Abilities.Models;

namespace PokeGame.Api.Models.Ability;

public record SearchAbilitiesParameters : SearchParameters
{
  public virtual SearchAbilitiesPayload ToPayload()
  {
    SearchAbilitiesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out AbilitySort field))
      {
        payload.Sort.Add(new AbilitySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
