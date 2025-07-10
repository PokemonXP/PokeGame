namespace PokeGame.Core.Pokemons.Models;

public record PowerPointsModel
{
  public int Current { get; set; }
  public int Maximum { get; set; }
  public int Reference { get; set; }

  public PowerPointsModel()
  {
  }

  public PowerPointsModel(int current, int maximum, int reference)
  {
    Current = current;
    Maximum = maximum;
    Reference = reference;
  }
}
