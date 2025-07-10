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
