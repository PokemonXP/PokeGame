using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Api.Models.Game;

public record ExperienceSummary
{
  public int Current { get; set; }
  public int Minimum { get; set; }
  public int Maximum { get; set; }
  public int ToNextLevel { get; set; }
  public double Percentage { get; set; }

  public ExperienceSummary()
  {
  }

  public ExperienceSummary(PokemonModel pokemon)
  {
    Current = pokemon.Experience;
    if (pokemon.Level > 1)
    {
      Minimum = ExperienceTable.Instance.GetMaximumExperience(pokemon.Form.Variety.Species.GrowthRate, pokemon.Level - 1);
    }
    Maximum = pokemon.MaximumExperience;
    ToNextLevel = pokemon.ToNextLevel;
    if (Minimum < Maximum)
    {
      Percentage = (Current - Minimum) / (double)(Maximum - Minimum);
    }
  }
}
