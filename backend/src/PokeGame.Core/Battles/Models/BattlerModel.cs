using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Battles.Models;

public record BattlerModel
{
  public PokemonModel Pokemon { get; set; } = new();
  public bool IsActive { get; set; }
}
