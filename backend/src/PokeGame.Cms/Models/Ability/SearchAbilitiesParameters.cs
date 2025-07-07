using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using PokeGame.Core.Abilities.Models;

namespace PokeGame.Cms.Models.Ability;

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
