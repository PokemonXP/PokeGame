namespace PokeGame.Core.Pokemons;

public record PokemonSize : IPokemonSize
{
  public byte Height { get; }
  public byte Weight { get; }

  public PokemonSize()
  {
  }

  public PokemonSize(byte height, byte weight)
  {
    Height = height;
    Weight = weight;
  }

  public PokemonSize(IPokemonSize size) : this(size.Height, size.Weight)
  {
  }
}
