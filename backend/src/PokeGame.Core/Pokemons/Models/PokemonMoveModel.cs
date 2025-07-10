using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Pokemons.Models;

public record PokemonMoveModel
{
  public MoveModel Move { get; set; } = new();

  public int? Position { get; set; }
  public PowerPointsModel PowerPoints { get; set; } = new();
  public bool IsMastered { get; set; }

  public int Level { get; set; }
  public bool TechnicalMachine { get; set; }
}

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
