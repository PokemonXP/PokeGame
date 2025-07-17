using Krakenar.Contracts.Search;

namespace PokeGame.Core.Moves.Models;

public record SearchMovesPayload : SearchPayload
{
  public PokemonType? Type { get; set; }
  public MoveCategory? Category { get; set; }

  public new List<MoveSortOption> Sort { get; set; } = [];
}
