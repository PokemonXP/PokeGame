namespace PokeGame.Core.Pokemons;

public interface IPokemonSize
{
  byte Height { get; }
  byte Weight { get; }
  PokemonSizeCategory Category { get; }
}
