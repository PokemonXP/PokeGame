using PokeGame.Core.Abilities.Models;

namespace PokeGame.Api.Models.Game;

public record AbilitySummary
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AbilitySummary() : this(string.Empty)
  {
  }

  public AbilitySummary(string name)
  {
    Name = name;
  }

  public AbilitySummary(AbilityModel ability)
  {
    Name = ability.DisplayName ?? ability.UniqueName;
    Description = ability.Description;
  }
}
